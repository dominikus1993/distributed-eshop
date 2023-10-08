using AutoFixture.Xunit2;

using Catalog.Core.Model;
using Catalog.Infrastructure.Model;

using OpenSearch.Client;

using Shouldly;

using Xunit;

namespace Catalog.Tests.Infrastructure.Model;

public class OpenSearchProductTests
{
    [Theory]
    [AutoData]
    internal void TestToProduct(OpenSearchProduct product)
    {
        product.ProductId = Guid.NewGuid();
        var subject = product.ToProduct();

        subject.ShouldNotBeNull();
        subject.ProductName.ShouldBe(new ProductName(product.Name));
        subject.ProductDescription.ShouldBe(new ProductDescription(product.Description));
        subject.AvailableQuantity.ShouldBe(new AvailableQuantity(product.AvailableQuantity));
        subject.Price.ShouldBe(new ProductPrice(product.Price, product.PromotionalPrice));
    }
    
    [Theory]
    [AutoData]
    public void TestFromProduct(Product product)
    {
        var subject = new OpenSearchProduct(product);

        subject.ShouldNotBeNull();
        subject.ProductId.ToString().ShouldBe(product.Id.Value.ToString());
        subject.Name.ShouldBe(product.ProductName.Name);
        subject.Description.ShouldBe(product.ProductDescription.Description);
        subject.AvailableQuantity.ShouldBe(product.AvailableQuantity.Value);
        subject.Price.ShouldBe(product.Price.CurrentPrice.Value);
        subject.PromotionalPrice.ShouldBe(product.Price.PromotionalPrice!.Value);
    }
}