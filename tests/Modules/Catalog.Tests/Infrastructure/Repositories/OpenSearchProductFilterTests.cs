using System.Security.Cryptography;

using AutoFixture.Xunit2;

using Catalog.Core.Model;
using Catalog.Core.Repository;
using Catalog.Infrastructure.Model;
using Catalog.Infrastructure.Repositories;
using Catalog.Tests.Fixtures;

using Shouldly;

using Xunit;

namespace Catalog.Tests.Infrastructure.Repositories;

[UsesVerify]
public sealed class OpenSearchProductFilterTests : IAsyncLifetime, IClassFixture<OpenSearchFixture>
{
    private readonly OpenSearchFixture _fixture;
    private readonly IProductFilter _productFilter;
    private readonly IProductsWriter _productsWriter;
    
    public OpenSearchProductFilterTests(OpenSearchFixture fixture)
    {
        _fixture = fixture;
        _productsWriter = fixture.ProductsWriter;
        _productFilter = fixture.ProductFilter;
    }

    [Theory]
    [InlineAutoData]
    public async Task FilterProductsWithoutAnyPredicates(Product[] products)
    {
        // Arrange
        var res = await _productsWriter.AddProducts(products);
        Assert.True(res.IsSuccess);
        
        // Act
        
        var subject = await _productFilter.FilterProducts(new Filter() { PageSize = products.Length, Page = 1 });

        subject.ShouldNotBeNull();
        subject.IsEmpty.ShouldBeFalse();
        subject.Count.ShouldBe((uint)products.Length);
        subject.Total.ShouldBe((uint)products.Length);
        
    }
    
    [Theory]
    [InlineAutoData]
    public async Task ReadFilterProductsWhenNoExistsShouldReturnNull(string query)
    {
        // Act

        var subject = await _productFilter.FilterProducts(new Filter() { Query = query });

        subject.ShouldNotBeNull();
        subject.IsEmpty.ShouldBeTrue();
    }


    [Fact]
    public async Task FilterProductWhenNiveaProductExistsShouldReturnProductsWithNameOrDescriptionContainsNiveaKeyword()
    {
        // Arrange 
        var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription("nivea"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
        var product2 = new Product(ProductId.New(), new ProductName("Nivea xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
        var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
        await _productsWriter.AddProducts(new[] { product1, product2, product3 });
        // Act

        var subject = await _productFilter.FilterProducts(new Filter() { Query = "nivea" });
        
        // Assert
        subject.ShouldNotBeNull();
        subject.Data.ShouldNotBeEmpty();
    
        await Verify(subject);
    }
    
    // [Theory]
    // [AutoData]
    // public async Task FilterProductWhenNiveaProductExistsShouldReturnProductsWithNameOrDescriptionContainsNiveaTag(string tag)
    // {
    //     // Arrange 
    //     using var cts = new CancellationTokenSource();
    //     cts.CancelAfter(TimeSpan.FromSeconds(30));
    //     var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription("nivea"),
    //         new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), new Tags(new []{ new Tag(tag)}));
    //     var product2 = new Product(ProductId.New(), new ProductName("Nivea xDDD"), new ProductDescription("xDDD"),
    //         new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
    //     var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
    //         new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
    //     await _productsWriter.AddProducts(new[] { product1, product2, product3 }, cts.Token);
    //     // Act
    //     
    //     var subject = await _productFilter.FilterProducts(new Filter() { Tag = tag } , cts.Token).ToListAsync(cancellationToken: cts.Token);
    //     
    //     // Assert
    //     subject.ShouldNotBeNull();
    //     subject.ShouldNotBeEmpty();
    //
    //     subject.Count.ShouldBe(1);
    //     subject[0].Id.ShouldBe(product1.Id);
    // }
    //
    // [Theory]
    // [AutoData]
    // public async Task FilterProductWhenNiveaProductNotExistsShouldReturnEmptyCollectionNiveaTag(Tags tags, string tag2)
    // {
    //     // Arrange 
    //     using var cts = new CancellationTokenSource();
    //     cts.CancelAfter(TimeSpan.FromSeconds(30));
    //     var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription("nivea"),
    //         new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), tags);
    //     var product2 = new Product(ProductId.New(), new ProductName("Nivea xDDD"), new ProductDescription("xDDD"),
    //         new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
    //     var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
    //         new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
    //     await _productsWriter.AddProducts(new[] { product1, product2, product3 }, cts.Token);
    //     // Act
    //     
    //     var subject = await _productFilter.FilterProducts(new Filter() { Tag = tag2 } , cts.Token).ToListAsync(cancellationToken: cts.Token);
    //     
    //     // Assert
    //     subject.ShouldNotBeNull();
    //     subject.ShouldBeEmpty();
    // }
    //
    // [Fact]
    // public async Task FilterProductWhenProductsInPriceConditionExistsShouldReturnProductsWithPriceCondition()
    // {
    //     // Arrange 
    //     using var cts = new CancellationTokenSource();
    //     cts.CancelAfter(TimeSpan.FromSeconds(30));
    //     
    //     var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription("nivea"),
    //         new ProductPrice(new Price(5m), new Price(1m)), new AvailableQuantity(10), Tags.Empty);
    //     var product2 = new Product(ProductId.New(), new ProductName("Nivea xDDD"), new ProductDescription("xDDD"),
    //         new ProductPrice(new Price(11m)), new AvailableQuantity(10), Tags.Empty);
    //     var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
    //         new ProductPrice(new Price(20m), new Price(20m)), new AvailableQuantity(10), Tags.Empty);
    //     await _productsWriter.AddProducts(new[] { product1, product2, product3 }, cts.Token);
    //     // Act
    //     
    //     var subject = await _productFilter.FilterProducts(new Filter() { PriceFrom = 2m, PriceTo = 12m } , cts.Token).ToListAsync(cancellationToken: cts.Token);
    //     
    //     // Assert
    //     subject.ShouldNotBeNull();
    //     subject.ShouldNotBeEmpty();
    //     subject.Count.ShouldBe(1);
    //     
    //     await Verify(subject);
    // }
    //
    // [Fact]
    // public async Task FilterProductWhenNiveaProductExistsShouldReturnOneProductWithNameOrDescriptionContainsNiveaKeyword()
    // {
    //     // Arrange 
    //     using var cts = new CancellationTokenSource();
    //     cts.CancelAfter(TimeSpan.FromSeconds(30));
    //     
    //     var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription("nivea"),
    //         new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
    //     var product2 = new Product(ProductId.New(), new ProductName("Nivea xDDD"), new ProductDescription("xDDD"),
    //         new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
    //     var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
    //         new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
    //     await _productsWriter.AddProducts(new[] { product1, product2, product3 }, cts.Token);
    //     // Act
    //     
    //     var subject = await _productFilter.FilterProducts(new Filter() { Query = "nivea", PageSize = 1 } , cts.Token).ToListAsync(cancellationToken: cts.Token);
    //     
    //     // Assert
    //     subject.ShouldNotBeNull();
    //     subject.ShouldNotBeEmpty();
    //     subject.Count.ShouldBe(1);
    //     
    //     await Verify(subject);
    // }
    //
    // [Fact]
    // public async Task FilterProductWhenNiveaProductExistsShouldReturnOneProductWithNameOrDescriptionContainsNiveaKeywordOnTheSecondPage()
    // {
    //     // Arrange 
    //     using var cts = new CancellationTokenSource();
    //     cts.CancelAfter(TimeSpan.FromSeconds(30));
    //     
    //     var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription("nivea"),
    //         new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
    //     var product2 = new Product(ProductId.New(), new ProductName("Nivea xDDD"), new ProductDescription("xDDD"),
    //         new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
    //     var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
    //         new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
    //     await _productsWriter.AddProducts(new[] { product1, product2, product3 }, cts.Token);
    //     // Act
    //     
    //     var subject = await _productFilter.FilterProducts(new Filter() { Query = "nivea", PageSize = 1, Page = 2 } , cts.Token).ToListAsync(cancellationToken: cts.Token);
    //     
    //     // Assert
    //     subject.ShouldNotBeNull();
    //     subject.ShouldNotBeEmpty();
    //     subject.Count.ShouldBe(1);
    //     
    //     await Verify(subject);
    //}

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return _fixture.Clean();
    }
}