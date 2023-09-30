using System.Security.Cryptography;

using AutoFixture.Xunit2;

using Catalog.Core.Model;
using Catalog.Core.Repository;
using Catalog.Infrastructure.Repositories;
using Catalog.Tests.Fixtures;

using Shouldly;

using Xunit;

namespace Catalog.Tests.Infrastructure.Repositories;

public class OpenSearchProductsWriterTests : IClassFixture<OpenSearchFixture>
{
    private readonly OpenSearchFixture _openSearchFixture;
    private readonly IProductReader _productReader;
    private readonly IProductsWriter _productsWriter;
    public OpenSearchProductsWriterTests(OpenSearchFixture openSearchFixture)
    {
        _openSearchFixture = openSearchFixture;
        _productsWriter = openSearchFixture.ProductsWriter;
        _productReader = openSearchFixture.ProductReader;
    }

    [Theory]
    [InlineAutoData]
    public async Task WriteProductTest(Product product)
    {
        // Act
        var subject = await _productsWriter.AddProduct(product);
                    
        subject.ShouldNotBeNull();
        subject.IsSuccess.ShouldBeTrue();
    }
    
    [Theory]
    [InlineAutoData]
    public async Task WriteProductAndRead(Product product)
    {
        // Act
        var subject = await _productsWriter.AddProduct(product);
        
        subject.ShouldNotBeNull();
        subject.IsSuccess.ShouldBeTrue();
        
        var productFromDb = await _productReader.GetById(product.Id);
        
        productFromDb.ShouldBeEquivalentTo(product);
    }
    
    [Theory]
    [InlineAutoData]
    public async Task WriteProductTwoTimesTest(Product product, ProductName newProductName)
    {
        // Act
        var subject = await _productsWriter.AddProduct(product);
        
        subject.ShouldNotBeNull();
        subject.IsSuccess.ShouldBeTrue();
    
        var newProduct = product with { ProductName = newProductName };
        subject = await _productsWriter.AddProduct(newProduct);
        
        subject.ShouldNotBeNull();
        subject.IsSuccess.ShouldBeTrue();
        
        var productFromDb = await _productReader.GetById(product.Id);
        
        productFromDb.ShouldBeEquivalentTo(newProduct);
    }
}