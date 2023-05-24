using System.Security.Cryptography;

using AutoFixture.Xunit2;

using Catalog.Core.Model;
using Catalog.Core.Repository;
using Catalog.Infrastructure.Repositories;
using Catalog.Tests.Fixtures;

using Shouldly;

using Xunit;

namespace Catalog.Tests.Infrastructure.Repositories;

[Collection(nameof(PostgresSqlFixtureCollectionTests)), UsesVerify]
public sealed class MartenProductFilterTests : IDisposable
{
    private readonly PostgresSqlFixture _postgresSqlFixture;
    private readonly IProductFilter _productFilter;
    private readonly IProductsWriter _productsWriter;
    public MartenProductFilterTests(PostgresSqlFixture postgresSqlFixture)
    {
        _postgresSqlFixture = postgresSqlFixture;
        _productsWriter = new EfCoreProductsWriter(_postgresSqlFixture.DbContextFactory);
        _productFilter = new EfCoreProductFilter(_postgresSqlFixture.DbContextFactory);
    }

    [Theory]
    [InlineAutoData]
    public async Task ReadFilterProductsWhenNoExistsShouldReturnNull(string query)
    {
        // Arrange 
        var repo = new EfCoreProductFilter(_postgresSqlFixture.DbContextFactory);
        
        // Act

        var subject = await repo.FilterProducts(new Filter() { Query = "nivea"}).ToListAsync();

        subject.ShouldNotBeNull();
        subject.ShouldBeEmpty();
    }


    [Fact]
    public async Task FilterProductWhenNiveaProductExistsShouldReturnProductsWithNameOrDescriptionContainsNiveaKeyword()
    {
        // Arrange 
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));
        
        var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription("nivea"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
        var product2 = new Product(ProductId.New(), new ProductName("Nivea xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
        var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
        await _productsWriter.AddProducts(new[] { product1, product2, product3 }, cts.Token);
        // Act
        
        var subject = await _productFilter.FilterProducts(new Filter() { Query = "nivea"} , cts.Token).ToListAsync(cancellationToken: cts.Token);
        
        // Assert
        subject.ShouldNotBeNull();
        subject.ShouldNotBeEmpty();

        await Verify(subject);
    }
    
    [Theory]
    [AutoData]
    public async Task FilterProductWhenNiveaProductExistsShouldReturnProductsWithNameOrDescriptionContainsNiveaTag(string tag)
    {
        // Arrange 
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));
        var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription("nivea"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), new Tags(new []{ new Tag(tag)}));
        var product2 = new Product(ProductId.New(), new ProductName("Nivea xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
        var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
        await _productsWriter.AddProducts(new[] { product1, product2, product3 }, cts.Token);
        // Act
        
        var subject = await _productFilter.FilterProducts(new Filter() { Tag = tag } , cts.Token).ToListAsync(cancellationToken: cts.Token);
        
        // Assert
        subject.ShouldNotBeNull();
        subject.ShouldNotBeEmpty();

        subject.Count.ShouldBe(1);
        subject[0].Id.ShouldBe(product1.Id);
    }
    
    [Theory]
    [AutoData]
    public async Task FilterProductWhenNiveaProductNotExistsShouldReturnEmptyCollectionNiveaTag(Tags tags, string tag2)
    {
        // Arrange 
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));
        var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription("nivea"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), tags);
        var product2 = new Product(ProductId.New(), new ProductName("Nivea xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
        var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
        await _productsWriter.AddProducts(new[] { product1, product2, product3 }, cts.Token);
        // Act
        
        var subject = await _productFilter.FilterProducts(new Filter() { Tag = tag2 } , cts.Token).ToListAsync(cancellationToken: cts.Token);
        
        // Assert
        subject.ShouldNotBeNull();
        subject.ShouldBeEmpty();
    }
    
    [Fact]
    public async Task FilterProductWhenProductsInPriceConditionExistsShouldReturnProductsWithPriceCondition()
    {
        // Arrange 
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));
        
        var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription("nivea"),
            new ProductPrice(new Price(5m), new Price(1m)), new AvailableQuantity(10), Tags.Empty);
        var product2 = new Product(ProductId.New(), new ProductName("Nivea xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(11m)), new AvailableQuantity(10), Tags.Empty);
        var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(20m), new Price(20m)), new AvailableQuantity(10), Tags.Empty);
        await _productsWriter.AddProducts(new[] { product1, product2, product3 }, cts.Token);
        // Act
        
        var subject = await _productFilter.FilterProducts(new Filter() { PriceFrom = 2m, PriceTo = 12m } , cts.Token).ToListAsync(cancellationToken: cts.Token);
        
        // Assert
        subject.ShouldNotBeNull();
        subject.ShouldNotBeEmpty();
        subject.Count.ShouldBe(1);
        
        await Verify(subject);
    }
    
    [Fact]
    public async Task FilterProductWhenNiveaProductExistsShouldReturnOneProductWithNameOrDescriptionContainsNiveaKeyword()
    {
        // Arrange 
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));
        
        var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription("nivea"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
        var product2 = new Product(ProductId.New(), new ProductName("Nivea xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
        var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
        await _productsWriter.AddProducts(new[] { product1, product2, product3 }, cts.Token);
        // Act
        
        var subject = await _productFilter.FilterProducts(new Filter() { Query = "nivea", PageSize = 1 } , cts.Token).ToListAsync(cancellationToken: cts.Token);
        
        // Assert
        subject.ShouldNotBeNull();
        subject.ShouldNotBeEmpty();
        subject.Count.ShouldBe(1);
        
        await Verify(subject);
    }
    
    [Fact]
    public async Task FilterProductWhenNiveaProductExistsShouldReturnOneProductWithNameOrDescriptionContainsNiveaKeywordOnTheSecondPage()
    {
        // Arrange 
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));
        
        var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription("nivea"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
        var product2 = new Product(ProductId.New(), new ProductName("Nivea xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
        var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
        await _productsWriter.AddProducts(new[] { product1, product2, product3 }, cts.Token);
        // Act
        
        var subject = await _productFilter.FilterProducts(new Filter() { Query = "nivea", PageSize = 1, Page = 2 } , cts.Token).ToListAsync(cancellationToken: cts.Token);
        
        // Assert
        subject.ShouldNotBeNull();
        subject.ShouldNotBeEmpty();
        subject.Count.ShouldBe(1);
        
        await Verify(subject);
    }
    
    public void Dispose()

    {
        _postgresSqlFixture.DbContext.Products.RemoveRange(_postgresSqlFixture.DbContext.Products);
        _postgresSqlFixture.DbContext.SaveChanges();
    }
}