using Catalog.Core.Model;
using Catalog.Infrastructure.Model;

using Shouldly;

using Xunit;

namespace Catalog.Tests.Infrastructure.Model;

public class EfProductTests
{
    [Fact]
    public void TestToProduct()
    {
        var efproduct = new EfProduct()
        {
            ProductId = ProductId.New(),
            AvailableQuantity = 1,
            Price = 10m,
            PromotionalPrice = 5m,
            Description = "SomeDesc",
            Name = "MyProduct"
        };
        var subject = efproduct.ToProduct();

        subject.ShouldNotBeNull();
        subject.Id.ShouldBe(efproduct.ProductId);
        subject.ProductName.ShouldBe(new ProductName(efproduct.Name));
        subject.ProductDescription.ShouldBe(new ProductDescription(efproduct.Description));
        subject.AvailableQuantity.ShouldBe(new AvailableQuantity(efproduct.AvailableQuantity));
        subject.Price.ShouldBe(new ProductPrice(efproduct.Price, efproduct.PromotionalPrice));
    }
    
    [Fact]
    public void TestFromProduct()
    {
        var productId = ProductId.New();
        var product = new Product(productId, new ProductName("xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10));
        var subject = new EfProduct(product);

        subject.ShouldNotBeNull();
        subject.ProductId.ShouldBe(product.Id);
        subject.Name.ShouldBe(product.ProductName.Name);
        subject.Description.ShouldBe(product.ProductDescription.Description);
        subject.AvailableQuantity.ShouldBe(product.AvailableQuantity.Value);
        subject.Price.ShouldBe(product.Price.CurrentPrice.Value);
        subject.PromotionalPrice.ShouldBe(product.Price.PromotionalPrice!.Value);
    }
}