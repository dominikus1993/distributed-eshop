using Catalog.Core.Model;

namespace Catalog.Core.Dto;

public sealed class ProductDto
{
    public int ProductId { get; init; }
    public string Description { get; init; }
    
    private ProductDto(int productId, string description)
    {
        ProductId = productId;
        Description = description;
    }

    public static ProductDto Create(Product product)
    {
        return new ProductDto(product.Id.Value, product.Description.Description);
    }
}