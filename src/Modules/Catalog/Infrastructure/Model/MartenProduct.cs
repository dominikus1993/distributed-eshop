using Catalog.Core.Model;

namespace Catalog.Infrastructure.Model;

public sealed class MartenProduct
{

    public int ProductId { get; set; }
    public MartenProduct()
    {
        
    }

    public MartenProduct(Product product)
    {
        
    }

    public Product ToProduct()
    {
        throw new NotImplementedException();
    }
}