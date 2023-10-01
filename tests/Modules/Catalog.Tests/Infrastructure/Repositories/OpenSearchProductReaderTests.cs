using System.Security.Cryptography;

using AutoFixture.Xunit2;

using Catalog.Core.Model;
using Catalog.Core.Repository;
using Catalog.Infrastructure.Repositories;
using Catalog.Tests.Fixtures;

using Shouldly;

using Xunit;

namespace Catalog.Tests.Infrastructure.Repositories;

public sealed class OpenSearchProductReaderTests : IClassFixture<OpenSearchFixture>
{
    private readonly OpenSearchFixture _fixture;
    private readonly IProductReader _productReader;
    private readonly IProductsWriter _productsWriter;
    public OpenSearchProductReaderTests(OpenSearchFixture fixture)
    {
        _fixture = fixture;
        _productsWriter = _fixture.ProductsWriter;
        _productReader = _fixture.ProductReader;
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
        // Act
        
        var subject = await _productReader.GetByIds(productIds).ToListAsync();
        
        // Assert
        subject.ShouldNotBeNull();
        subject.ShouldBeEmpty();
    }
    
    [Theory]
    [InlineAutoData]
    public async Task ReadProductsByIdsWhenNoExistsShouldReturnEmptyEnumerable(IEnumerable<ProductId> productIds)
    {
        // Act
        
        var subject = await _productReader.GetByIds(productIds).ToListAsync();
        
        // Assert
        subject.ShouldNotBeNull();
        subject.ShouldBeEmpty();
    }
    
    [Theory]
    [InlineAutoData]
    public async Task ReadProductByIdWhenExistsShouldReturnProduct(Product product)
    {
        // Act

        await _productsWriter.AddProduct(product);
        var subject = await _productReader.GetById(product.Id);
        
        subject.ShouldNotBeNull();
        subject.Id.ShouldBe(product.Id);
        subject.ProductName.ShouldBe(product.ProductName);
        subject.Price.ShouldBe(product.Price);
        subject.AvailableQuantity.ShouldBe(product.AvailableQuantity);
        subject.ProductDescription.ShouldBe(product.ProductDescription);
    }
    
    [Theory]
    [AutoData]
    public async Task ReadProductsByIdsWhenExistsShouldReturnProducts(Product[] products)
    {

        // Act

        await _productsWriter.AddProducts(products);
        var subject = await _productReader.GetByIds(products.Select(x => x.Id)).ToArrayAsync();
        
        subject.ShouldNotBeNull();
        subject.ShouldNotBeEmpty();
        subject.Length.ShouldBe(products.Length);
        subject.ShouldBeEquivalentTo(products);
    }
}