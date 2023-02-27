using System.Security.Cryptography;

using AutoFixture.Xunit2;

using Catalog.Core.Model;
using Catalog.Core.Repository;
using Catalog.Infrastructure.Repositories;
using Catalog.Tests.Fixtures;

using Shouldly;

using Xunit;

namespace Catalog.Tests.Infrastructure.Repositories;

[Collection(nameof(PostgresSqlFixtureCollectionTests))]
public class MartenProductsWriterTests
{
    private readonly PostgresSqlFixture _postgresSqlFixture;
    private readonly IProductReader _productReader;
    private readonly IProductsWriter _productsWriter;
    public MartenProductsWriterTests(PostgresSqlFixture postgresSqlFixture)
    {
        _postgresSqlFixture = postgresSqlFixture;
        _productsWriter = new EfCoreProductsWriter(_postgresSqlFixture.DbContextFactory);
        _productReader = new EfCoreProductReader(_postgresSqlFixture.DbContextFactory);
    }

    [Theory]
    [InlineAutoData]
    public async Task WriteProductTest(Product product)
    {
        // Act
        var subject = await _productsWriter.AddProduct(product);
                    
        subject.ShouldNotBeNull();
        subject.IsT0.ShouldBeTrue();
    }
    
    [Theory]
    [InlineAutoData]
    public async Task WriteProductTwoTimesTest(Product product, ProductName newProductName)
    {
        // Act
        var subject = await _productsWriter.AddProduct(product);
        
        subject.ShouldNotBeNull();
        subject.IsT0.ShouldBeTrue();

        var newProduct = product with { ProductName = newProductName };
        subject = await _productsWriter.AddProduct(newProduct);
        
        subject.ShouldNotBeNull();
        subject.IsT0.ShouldBeTrue();
        
        var productFromDb = await _productReader.GetById(product.Id);
        
        productFromDb.ShouldBeEquivalentTo(newProduct);
    }
}