using System.Security.Cryptography;

using Catalog.Core.Model;
using Catalog.Infrastructure.Repositories;
using Catalog.Tests.Fixtures;

using Shouldly;

using Xunit;

namespace Catalog.Tests.Infrastructure.Repositories;

public class MartenProductReaderTests : IClassFixture<PostgresSqlFixture>
{
    private readonly PostgresSqlFixture _postgresSqlFixture;

    public MartenProductReaderTests(PostgresSqlFixture postgresSqlFixture)
    {
        _postgresSqlFixture = postgresSqlFixture;
    }

    [Fact]
    public async Task ReadProductByIdWhenNoExistsShouldReturnNull()
    {
        // Arrange 
        var repo = new EfCoreProductReader(_postgresSqlFixture.DbContext);
        
        // Act

        var subject = await repo.GetById(ProductId.New());
        
        subject.ShouldBeNull();
    }
    
    [Fact]
    public async Task ReadProductByIdWhenExistsShouldReturnProduct()
    {
        // Arrange 
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));
        var productId = ProductId.New();
        var product = new Product(productId, new ProductName("xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10));
        
        var repo = new EfCoreProductReader(_postgresSqlFixture.DbContext);
        var writer = new EfCoreProductsWriter(_postgresSqlFixture.DbContext);
        // Act

        await writer.AddProduct(product, cts.Token);
        var subject = await repo.GetById(productId, cts.Token);
        
        subject.ShouldNotBeNull();
        subject.Id.ShouldBe(productId);
        subject.ProductName.ShouldBe(product.ProductName);
        subject.Price.ShouldBe(product.Price);
        subject.AvailableQuantity.ShouldBe(product.AvailableQuantity);
        subject.ProductDescription.ShouldBe(product.ProductDescription);
    }
}