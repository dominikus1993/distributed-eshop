namespace Warehouse.Core.Model;

public readonly record struct ProductId(Guid Value)
{
    public static ProductId From(Guid id) => new ProductId(id);
    
    public static ProductId New() => new ProductId(Guid.NewGuid());
    
    public static implicit operator Guid (ProductId id) => id.Value;
    
    public static implicit operator ProductId (Guid id) => new ProductId(id);
}