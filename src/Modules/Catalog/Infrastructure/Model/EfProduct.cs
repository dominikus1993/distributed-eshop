using Catalog.Core.Model;

namespace Catalog.Infrastructure.Model;

public sealed class EfProduct
{
    public ProductId ProductId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    
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