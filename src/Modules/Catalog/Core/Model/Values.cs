namespace Catalog.Core.Model;

public readonly record struct ProductId(Guid Value)
{
    public static ProductId From(Guid id) => new ProductId(id);
    
    public static ProductId New() => new ProductId(Guid.NewGuid());
}