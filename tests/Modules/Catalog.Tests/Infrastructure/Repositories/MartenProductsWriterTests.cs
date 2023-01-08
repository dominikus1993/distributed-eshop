using System.Security.Cryptography;

using Catalog.Core.Model;
using Catalog.Infrastructure.Repositories;
using Catalog.Tests.Fixtures;

using Shouldly;

using Xunit;

namespace Catalog.Tests.Infrastructure.Repositories;

[Collection(nameof(ProductContextContextCollection))]
public class MartenProductsWriterTests
{
    private readonly PostgresSqlFixture _postgresSqlFixture;

    public MartenProductsWriterTests(PostgresSqlFixture postgresSqlFixture)
    {
        _postgresSqlFixture = postgresSqlFixture;
    }

    [Fact]
    public async Task WriteProductTest()
    {
        // Arrange 
        var repo = new EfCoreProductsWriter(_postgresSqlFixture.DbContext);
        
        // Act
        var productId =  ProductId.New();
        var product = new Product(productId, new ProductName("xD"), new ProductDescription("xDD"), new ProductPrice(21.37m, 21.36m), new AvailableQuantity(1));
        var subject = await repo.AddProduct(product);
                    
        subject.ShouldNotBeNull();
        subject.IsT0.ShouldBeTrue();
    }
}