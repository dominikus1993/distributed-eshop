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
        subject.Description.ShouldBe(new ProductDescription(efproduct.Description));
        subject.AvailableQuantity.ShouldBe(new AvailableQuantity(efproduct.AvailableQuantity));
        subject.Price.ShouldBe(new ProductPrice(efproduct.Price, efproduct.PromotionalPrice));
    }
}