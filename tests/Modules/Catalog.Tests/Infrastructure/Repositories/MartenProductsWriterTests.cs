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
    private readonly IProductsWriter _productsWriter;
    public MartenProductsWriterTests(PostgresSqlFixture postgresSqlFixture)
    {
        _postgresSqlFixture = postgresSqlFixture;
        _productsWriter = new EfCoreProductsWriter(_postgresSqlFixture.DbContextFactory);
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
}