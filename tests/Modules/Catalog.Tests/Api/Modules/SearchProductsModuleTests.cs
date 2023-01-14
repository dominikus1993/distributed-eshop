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
public class SearchProductsModuleTests
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
        var writer = new EfCoreProductsWriter(_apiFixture.DbContextFactory);
        // Act
    
        await writer.AddProduct(product, cts.Token);
        
        var resp = await host.Scenario(s =>
        {
            s.Get.Url($"/api/products/{productId.Value}");
            s.StatusCodeShouldBeOk();
        });
        
        var subject = await resp.ReadAsJsonAsync<ProductResponse>();
        
        subject.ShouldNotBeNull();

        await Verify(subject);
    }
    //
    // [Fact]
    // public async Task ReadProductByIdsWhenExistsShouldReturnProduct()
    // {
    //     // Arrange 
    //     using var cts = new CancellationTokenSource();
    //     cts.CancelAfter(TimeSpan.FromSeconds(30));
    //     var productId = ProductId.New();
    //     var product = new Product(productId, new ProductName("xDDD"), new ProductDescription("xDDD"),
    //         new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10));
    //     
    //     var repo = new EfCoreProductReader(_postgresSqlFixture.DbContextFactory);
    //     var writer = new EfCoreProductsWriter(_postgresSqlFixture.DbContextFactory);
    //     // Act
    //
    //     await writer.AddProduct(product, cts.Token);
    //     var subject = await repo.GetByIds(new [] { productId } , cts.Token).ToListAsync(cancellationToken: cts.Token);
    //     
    //     subject.ShouldNotBeNull();
    //     subject.ShouldNotBeEmpty();
    //     subject.Count.ShouldBe(1);
    //     var productFromDb = subject[0];
    //     productFromDb.Id.ShouldBe(productId);
    //     productFromDb.ProductName.ShouldBe(product.ProductName);
    //     productFromDb.Price.ShouldBe(product.Price);
    //     productFromDb.AvailableQuantity.ShouldBe(product.AvailableQuantity);
    //     productFromDb.ProductDescription.ShouldBe(product.ProductDescription);
    // }
}