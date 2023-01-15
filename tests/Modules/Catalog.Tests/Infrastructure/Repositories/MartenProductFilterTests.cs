using System.Security.Cryptography;

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

    public MartenProductFilterTests(PostgresSqlFixture postgresSqlFixture)
    {
        _postgresSqlFixture = postgresSqlFixture;
    }

    [Fact]
    public async Task ReadFilterProductsWhenNoExistsShouldReturnNull()
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

        var repo = new EfCoreProductFilter(_postgresSqlFixture.DbContextFactory);
        var writer = new EfCoreProductsWriter(_postgresSqlFixture.DbContextFactory);
        var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription("nivea"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10));
        var product2 = new Product(ProductId.New(), new ProductName("Nivea xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10));
        var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10));
        await writer.AddProducts(new[] { product1, product2, product3 }, cts.Token);
        // Act
        
        var subject = await repo.FilterProducts(new Filter() { Query = "nivea"} , cts.Token).ToListAsync(cancellationToken: cts.Token);
        
        // Assert
        subject.ShouldNotBeNull();
        subject.ShouldNotBeEmpty();

        await Verify(subject);
    }
    
    [Fact]
    public async Task FilterProductWhenProductsInPriceConditionExistsShouldReturnProductsWithPriceCondition()
    {
        // Arrange 
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));

        var repo = new EfCoreProductFilter(_postgresSqlFixture.DbContextFactory);
        var writer = new EfCoreProductsWriter(_postgresSqlFixture.DbContextFactory);
        var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription("nivea"),
            new ProductPrice(new Price(5m), new Price(1m)), new AvailableQuantity(10));
        var product2 = new Product(ProductId.New(), new ProductName("Nivea xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(11m)), new AvailableQuantity(10));
        var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(20m), new Price(20m)), new AvailableQuantity(10));
        await writer.AddProducts(new[] { product1, product2, product3 }, cts.Token);
        // Act
        
        var subject = await repo.FilterProducts(new Filter() { PriceFrom = 2m, PriceTo = 12m } , cts.Token).ToListAsync(cancellationToken: cts.Token);
        
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

        var repo = new EfCoreProductFilter(_postgresSqlFixture.DbContextFactory);
        var writer = new EfCoreProductsWriter(_postgresSqlFixture.DbContextFactory);
        var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription("nivea"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10));
        var product2 = new Product(ProductId.New(), new ProductName("Nivea xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10));
        var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10));
        await writer.AddProducts(new[] { product1, product2, product3 }, cts.Token);
        // Act
        
        var subject = await repo.FilterProducts(new Filter() { Query = "nivea", PageSize = 1 } , cts.Token).ToListAsync(cancellationToken: cts.Token);
        
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

        var repo = new EfCoreProductFilter(_postgresSqlFixture.DbContextFactory);
        var writer = new EfCoreProductsWriter(_postgresSqlFixture.DbContextFactory);
        var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription("nivea"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10));
        var product2 = new Product(ProductId.New(), new ProductName("Nivea xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10));
        var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10));
        await writer.AddProducts(new[] { product1, product2, product3 }, cts.Token);
        // Act
        
        var subject = await repo.FilterProducts(new Filter() { Query = "nivea", PageSize = 1, Page = 2 } , cts.Token).ToListAsync(cancellationToken: cts.Token);
        
        // Assert
        subject.ShouldNotBeNull();
        subject.ShouldNotBeEmpty();
        subject.Count.ShouldBe(1);
        
        await Verify(subject);
    }

#pragma warning disable CA1816
#pragma warning disable CA1063
    public void Dispose()
#pragma warning restore CA1063
#pragma warning restore CA1816
    {
        _postgresSqlFixture.DbContext.Products.RemoveRange(_postgresSqlFixture.DbContext.Products);
        _postgresSqlFixture.DbContext.SaveChanges();
    }
}