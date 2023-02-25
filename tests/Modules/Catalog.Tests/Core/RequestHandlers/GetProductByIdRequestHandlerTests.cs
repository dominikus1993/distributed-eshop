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
    private readonly IProductsWriter _productsWriter;
    private readonly GetProductByIdRequestHandler _getProductByIdRequestHandler;

    public GetProductByIdRequestHandlerTests(PostgresSqlFixture postgresSqlFixture)
    {
        _postgresSqlFixture = postgresSqlFixture;
        _productReader = new EfCoreProductReader(_postgresSqlFixture.DbContextFactory);
        _productsWriter = new EfCoreProductsWriter(_postgresSqlFixture.DbContextFactory);
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
    
    [Theory]
    [InlineAutoData]
    public async Task ReadProductByIdWhenExistsShouldReturnProduct(Product product)
    {
        // Arrange 
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));

        // Act
        await _productsWriter.AddProduct(product, cts.Token);
        var subject = await _getProductByIdRequestHandler.Handle(new GetProductById(product.Id), default);
        
        subject.ShouldNotBeNull();
        subject.ProductId.ShouldBe(product.Id.Value);
        subject.Name.ShouldBe(product.ProductName.Name);
        subject.Price.ShouldBe(product.Price.CurrentPrice.Value);
        subject.AvailableQuantity.ShouldBe(product.AvailableQuantity.Value);
        subject.Description.ShouldBe(product.ProductDescription.Description);
    }
    
    [Theory]
    [InlineAutoData]
    public async Task ReadProductByIdsWhenExistsShouldReturnProduct(Product[] products)
    {
        // Arrange 
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));
        
        // Act
        await _productsWriter.AddProducts(products, cts.Token);
        var subject = await _productReader.GetByIds(products.Select(x => x.Id) , cts.Token).ToListAsync(cancellationToken: cts.Token);
        
        subject.ShouldNotBeNull();
        subject.ShouldNotBeEmpty();
        subject.Count.ShouldBe(products.Length);

        foreach (Product product in products)
        {
            subject.ShouldContain(product);
        }
    }
}