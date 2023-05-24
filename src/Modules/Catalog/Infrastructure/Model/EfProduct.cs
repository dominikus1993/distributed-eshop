using System.ComponentModel.DataAnnotations.Schema;

using Catalog.Core.Model;

using NpgsqlTypes;

namespace Catalog.Infrastructure.Model;

public sealed class EfProduct
{
    public ProductId ProductId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    public decimal? PromotionalPrice { get; set; }

    public decimal Price { get; set; }
    
    public int AvailableQuantity { get; set; }
    
#pragma warning disable CA1819
    [Column(TypeName = "jsonb")]
    public List<string>? Tags { get; set; }
#pragma warning restore CA1819

    public NpgsqlTsVector SearchVector { get; set; } = null!;
    
    public DateTimeOffset DateCreated { get; set; } = DateTimeOffset.UtcNow;

    public EfProduct()
    {
        
    }

    public EfProduct(Product product)
    {
        ProductId = product.Id;
        Description = product.ProductDescription.Description;
        Name = product.ProductName.Name;
        AvailableQuantity = product.AvailableQuantity.Value;
        Price = product.Price.CurrentPrice;
        PromotionalPrice = product.Price.PromotionalPrice;
        Tags = product.Tags?.Select(x => x.Name).ToList();
    }

    public Product ToProduct()
    {
        var tags = Tags?.Select(tag => new Tag(tag)).ToArray() ?? Array.Empty<Tag>();
        return new Product(ProductId, new ProductName(Name), new ProductDescription(Description),
            new ProductPrice(Price, PromotionalPrice), new AvailableQuantity(AvailableQuantity), new Tags(tags));
    }
}