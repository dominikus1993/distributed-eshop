namespace Catalog.Core.Model;

public readonly record struct ProductName(string Name);
public sealed record ProductDescription(string Description);

public readonly record struct Price(decimal Value);

public sealed record ProductPrice(Price CurrentPrice, Price? PromotionalPrice = null)
{
}

public readonly record struct AvailableQuantity(int Value);

public sealed record Product(ProductId Id, ProductName ProductName, ProductDescription Description, ProductPrice Price,
    AvailableQuantity AvailableQuantity)
{
}