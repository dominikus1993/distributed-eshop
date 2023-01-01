using Catalog.Core.Model;

using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Infrastructure.Model;

public sealed class MartenProduct
{
    [BsonId]
    public int ProductId { get; set; }
    public string? Name { get; set; }
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