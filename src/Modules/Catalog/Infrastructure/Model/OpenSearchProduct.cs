using Catalog.Core.Model;

using OpenSearch.Client;

namespace Catalog.Infrastructure.Model;

[OpenSearchType(IdProperty = nameof(ProductId))]
internal sealed class OpenSearchProduct
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    public decimal? PromotionalPrice { get; set; }

    public decimal Price { get; set; }
    
    public int AvailableQuantity { get; set; }
    
    public IReadOnlyList<string>? Tags { get; set; }
    
    
    public DateTimeOffset DateCreated { get; set; } = DateTimeOffset.UtcNow;

    public OpenSearchProduct()
    {
        
    }

    public OpenSearchProduct(Product product)
    {
        ProductId = product.Id;
        Description = product.ProductDescription.Description;
        Name = product.ProductName.Name;
        AvailableQuantity = product.AvailableQuantity.Value;
        Price = product.Price.CurrentPrice;
        PromotionalPrice = product.Price.PromotionalPrice;
        Tags = product.Tags?.Select(x => x.Name).ToArray();
    }

    public Product ToProduct()
    {
        var tags = Tags?.Select(tag => new Tag(tag)).ToArray() ?? Array.Empty<Tag>();
        return new Product(ProductId, new ProductName(Name), new ProductDescription(Description),
            new ProductPrice(Price, PromotionalPrice), new AvailableQuantity(AvailableQuantity), new Tags(tags));
    }
}