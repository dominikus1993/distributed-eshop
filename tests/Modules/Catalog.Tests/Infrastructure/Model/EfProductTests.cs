using Catalog.Core.Model;
using Catalog.Infrastructure.Model;

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
    }
}