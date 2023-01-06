using Catalog.Core.Model;

namespace Catalog.Core.Dto;

public sealed class ProductDto
{
    public Guid ProductId { get; init; }
    
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;

    public decimal? PromotionalPrice { get; init; }

    public decimal Price { get; init; }
    
    public int AvailableQuantity { get; init; }
    
    public ProductDto(Product product)
    {
        ProductId = product.Id.Value;
        Description = product.ProductDescription.Description;
        Name = product.ProductName.Name;
        Price = product.Price.CurrentPrice;
        PromotionalPrice = product.Price.PromotionalPrice;
        AvailableQuantity = product.AvailableQuantity.Value;
    }
}