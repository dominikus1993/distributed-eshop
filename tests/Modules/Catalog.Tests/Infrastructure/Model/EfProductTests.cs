using AutoFixture.Xunit2;

using Catalog.Core.Model;
using Catalog.Infrastructure.Model;

using Shouldly;

using Xunit;

namespace Catalog.Tests.Infrastructure.Model;

public class EfProductTests
{
    [Theory]
    [InlineAutoData()]
    public void TestToProduct(EfProduct efproduct)
    {
        var subject = efproduct.ToProduct();

        subject.ShouldNotBeNull();
        subject.Id.ShouldBe(efproduct.ProductId);
        subject.ProductName.ShouldBe(new ProductName(efproduct.Name));
        subject.ProductDescription.ShouldBe(new ProductDescription(efproduct.Description));
        subject.AvailableQuantity.ShouldBe(new AvailableQuantity(efproduct.AvailableQuantity));
        subject.Price.ShouldBe(new ProductPrice(efproduct.Price, efproduct.PromotionalPrice));
    }
    
    [Theory]
    [InlineAutoData()]
    public void TestFromProduct(Product product)
    {
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