using Catalog.Core.Model;

namespace Catalog.Infrastructure.Model;

public sealed class EfProduct
{
    public ProductId ProductId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    public decimal? PromotionalPrice { get; set; }

    public decimal Price { get; set; }
    
    public int AvailableQuantity { get; set; }
    
    public EfProduct()
    {
        
    }

    public EfProduct(Product product)
    {
        ProductId = product.Id;
        Description = product.Description.Description;
        Name = product.ProductName.Name;
    }

    public Product ToProduct()
    {
        throw new NotImplementedException();
    }
}