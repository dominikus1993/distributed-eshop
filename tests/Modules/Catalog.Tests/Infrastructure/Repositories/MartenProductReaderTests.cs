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
public class MartenProductReaderTests 
{
    private readonly PostgresSqlFixture _postgresSqlFixture;
    private readonly IProductReader _productReader;
    private readonly IProductsWriter _productsWriter;
    public MartenProductReaderTests(PostgresSqlFixture postgresSqlFixture)
    {
        _postgresSqlFixture = postgresSqlFixture;
        _productsWriter = new EfCoreProductsWriter(_postgresSqlFixture.DbContextFactory);
        _productReader = new EfCoreProductReader(_postgresSqlFixture.DbContextFactory);
    }

    [Theory]
    [InlineAutoData]
    public async Task ReadProductByIdWhenNoExistsShouldReturnNull(ProductId productId)
    {
      
        // Act
        var subject = await _productReader.GetById(productId);
        
        subject.ShouldBeNull();
    }
    
    [Theory]
    [InlineAutoData]
    public async Task ReadProductByIdsWhenNoExistsShouldReturnEmptyEnumerable(IEnumerable<ProductId> productIds)
    {
        // Arrange 
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));
        // Act
        
        var subject = await _productReader.GetByIds(productIds , cts.Token).ToListAsync(cancellationToken: cts.Token);
        
        // Assert
        subject.ShouldNotBeNull();
        subject.ShouldBeEmpty();
    }
    
    [Theory]
    [InlineAutoData]
    public async Task ReadProductsByIdsWhenNoExistsShouldReturnEmptyEnumerable(IEnumerable<ProductId> productIds)
    {
        // Arrange 
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));

        // Act
        
        var subject = await _productReader.GetByIds(productIds , cts.Token).ToListAsync(cancellationToken: cts.Token);
        
        // Assert
        subject.ShouldNotBeNull();
        subject.ShouldBeEmpty();
    }
    
    [Theory]
    [InlineAutoData]
    public async Task ReadProductByIdWhenExistsShouldReturnProduct(Product product)
    {
        // Arrange 
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));
        
        // Act

        await _productsWriter.AddProduct(product, cts.Token);
        var subject = await _productReader.GetById(product.Id, cts.Token);
        
        subject.ShouldNotBeNull();
        subject.Id.ShouldBe(product.Id);
        subject.ProductName.ShouldBe(product.ProductName);
        subject.Price.ShouldBe(product.Price);
        subject.AvailableQuantity.ShouldBe(product.AvailableQuantity);
        subject.ProductDescription.ShouldBe(product.ProductDescription);
    }
    
    [Theory]
    [InlineAutoData]
    public async Task ReadProductByIdsWhenExistsShouldReturnProduct(Product product)
    {
        // Arrange 
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));

        // Act

        await _productsWriter.AddProduct(product, cts.Token);
        var subject = await _productReader.GetByIds(new [] { product.Id } , cts.Token).ToListAsync(cancellationToken: cts.Token);
        
        subject.ShouldNotBeNull();
        subject.ShouldNotBeEmpty();
        subject.Count.ShouldBe(1);
        var productFromDb = subject[0];
        productFromDb.Id.ShouldBe(product.Id);
        productFromDb.ProductName.ShouldBe(product.ProductName);
        productFromDb.Price.ShouldBe(product.Price);
        productFromDb.AvailableQuantity.ShouldBe(product.AvailableQuantity);
        productFromDb.ProductDescription.ShouldBe(product.ProductDescription);
    }
}