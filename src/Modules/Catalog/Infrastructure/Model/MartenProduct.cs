using Catalog.Core.Model;

using Marten.Schema;

namespace Catalog.Infrastructure.Model;

public sealed class MartenProduct
{

    public int ProductId { get; set; }
    [FullTextIndex]
    public string? Name { get; set; }
    [FullTextIndex]
    public string? Description { get; set; }
    
    public MartenProduct()
    {
        
    }

    public MartenProduct(Product product)
    {
        ProductId = product.Id.Value;
        Description = product.Description.Description;
        Name = product.ProductName.Name;
    }

    public Product ToProduct()
    {
        throw new NotImplementedException();
    }
}