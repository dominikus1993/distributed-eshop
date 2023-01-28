using Alba;

using Catalog.Core.Dto;
using Catalog.Core.Model;
using Catalog.Core.RequestHandlers;
using Catalog.Core.Requests;
using Catalog.Infrastructure.Repositories;
using Catalog.Tests.Fixtures;

using Shouldly;

using Xunit;

namespace Catalog.Tests.Api.Modules;

public sealed class ProductResponse
{
    public Guid ProductId { get; init; }
    
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;

    public decimal? PromotionalPrice { get; init; }

    public decimal Price { get; init; }
    
    public int AvailableQuantity { get; init; }
}


[Collection(nameof(CatalogApiFixtureCollectionTest)), UsesVerify]
public sealed class SearchProductsModuleTests : IDisposable
{
    private readonly CatalogApiFixture _apiFixture;

    public SearchProductsModuleTests(CatalogApiFixture postgresSqlFixture)
    {
        _apiFixture = postgresSqlFixture;
    }

    [Fact]
    public async Task ReadProductByIdWhenNoExistsShouldReturnNull()
    {
        // Arrange 
        await using var host = await _apiFixture.GetHost();
        var productId = ProductId.New();
        // Act
        var resp = await host.Scenario(s =>
        {
            s.Get.Url($"/api/products/{productId.Value}");
            s.StatusCodeShouldBe(404);
        });
    }
    
    
    [Fact]
    public async Task ReadProductByIdWhenExistsShouldReturnProduct()
    {
        // Arrange 
        await using var host = await _apiFixture.GetHost();
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));
        var productId = ProductId.New();
        var product = new Product(productId, new ProductName("xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10));
        // Act
    
        await _apiFixture.ProductsWriter.AddProduct(product, cts.Token);
        
        var resp = await host.Scenario(s =>
        {
            s.Get.Url($"/api/products/{productId.Value}");
            s.StatusCodeShouldBeOk();
        });
        
        var subject = await resp.ReadAsJsonAsync<ProductResponse>();
        
        subject.ShouldNotBeNull();
        subject.ProductId.ShouldBe(productId.Value);
        await Verify(subject);
    }
    
    [Fact]
    public async Task SearchProductsWhenNoExists()
    {
        // Arrange 
        await using var host = await _apiFixture.GetHost();
        
        // Act
        await host.Scenario(s =>
        {
            s.Get.Url($"/api/products");
            s.StatusCodeShouldBe(204);
        }); ;
    }
    
    [Fact]
    public async Task SearchProductsWhenNiveaProductExistsShouldReturnProductsWithNameOrDescriptionContainsNiveaKeyword()
    {
        // Arrange 
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));

        await using var host = await _apiFixture.GetHost();

        var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription("nivea"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10));
        var product2 = new Product(ProductId.New(), new ProductName("Nivea xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10));
        var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10));
        await _apiFixture.ProductsWriter.AddProducts(new[] { product1, product2, product3 }, cts.Token);
        // Act
        
        var resp = await host.Scenario(s =>
        {
            s.Get.Url($"/api/products");
            s.StatusCodeShouldBeOk();
        });
        
        var subject = await resp.ReadAsJsonAsync<IReadOnlyCollection<ProductResponse>>();
        
        subject.ShouldNotBeNull();
        subject.ShouldNotBeEmpty();
        
        await Verify(subject);
    }

    public void Dispose()
    {
        _apiFixture.ProductsWriter.RemoveAllProducts().Wait();
    }
}