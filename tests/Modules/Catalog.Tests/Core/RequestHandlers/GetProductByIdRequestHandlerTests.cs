using AutoFixture.Xunit2;

using Catalog.Core.Model;
using Catalog.Core.Repository;
using Catalog.Core.RequestHandlers;
using Catalog.Core.Requests;
using Catalog.Infrastructure.Repositories;
using Catalog.Tests.Fixtures;

using Shouldly;

using Xunit;

namespace Catalog.Tests.Core.RequestHandlers;

[Collection(nameof(PostgresSqlFixtureCollectionTests))]
public class GetProductByIdRequestHandlerTests
{
    private readonly PostgresSqlFixture _postgresSqlFixture;
    private readonly IProductReader _productReader;
    private readonly GetProductByIdRequestHandler _getProductByIdRequestHandler;

    public GetProductByIdRequestHandlerTests(PostgresSqlFixture postgresSqlFixture)
    {
        _postgresSqlFixture = postgresSqlFixture;
        _productReader = new EfCoreProductReader(_postgresSqlFixture.DbContextFactory);
        _getProductByIdRequestHandler = new GetProductByIdRequestHandler(_productReader);
    }

    [Theory]
    [InlineAutoData]
    public async Task ReadProductByIdWhenNoExistsShouldReturnNull(GetProductById query)
    {
        // Act
        var subject = await _getProductByIdRequestHandler.Handle(query, default);
        
        subject.ShouldBeNull();
    }
    
    [Fact]
    // TODO Use other request handler
    public async Task ReadProductByIdsWhenNoExistsShouldReturnEmptyEnumerable()
    {
        // Arrange 
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));
        var productId = ProductId.New();
        // Act
        
        var subject = await _productReader.GetByIds(new [] { productId } , cts.Token).ToListAsync(cancellationToken: cts.Token);
        
        // Assert
        subject.ShouldNotBeNull();
        subject.ShouldBeEmpty();
    }
    
    [Fact]
    // TODO Use other request handler
    public async Task ReadProductsByIdsWhenNoExistsShouldReturnEmptyEnumerable()
    {
        // Arrange 
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));

        var repo = new EfCoreProductReader(_postgresSqlFixture.DbContextFactory);
        
        // Act
        
        var subject = await repo.GetByIds(new [] { ProductId.New(), ProductId.New() } , cts.Token).ToListAsync(cancellationToken: cts.Token);
        
        // Assert
        subject.ShouldNotBeNull();
        subject.ShouldBeEmpty();
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
        
        var repo = new EfCoreProductReader(_postgresSqlFixture.DbContextFactory);
        var writer = new EfCoreProductsWriter(_postgresSqlFixture.DbContextFactory);
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
    
    [Fact]
    public async Task ReadProductByIdsWhenExistsShouldReturnProduct()
    {
        // Arrange 
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));
        var productId = ProductId.New();
        var product = new Product(productId, new ProductName("xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10));
        
        var repo = new EfCoreProductReader(_postgresSqlFixture.DbContextFactory);
        var writer = new EfCoreProductsWriter(_postgresSqlFixture.DbContextFactory);
        // Act

        await writer.AddProduct(product, cts.Token);
        var subject = await repo.GetByIds(new [] { productId } , cts.Token).ToListAsync(cancellationToken: cts.Token);
        
        subject.ShouldNotBeNull();
        subject.ShouldNotBeEmpty();
        subject.Count.ShouldBe(1);
        var productFromDb = subject[0];
        productFromDb.Id.ShouldBe(productId);
        productFromDb.ProductName.ShouldBe(product.ProductName);
        productFromDb.Price.ShouldBe(product.Price);
        productFromDb.AvailableQuantity.ShouldBe(product.AvailableQuantity);
        productFromDb.ProductDescription.ShouldBe(product.ProductDescription);
    }
}