namespace Catalog.Core.Model;

public sealed record ProductDescription(string Title, string Description);

public readonly record struct Price(decimal Value);

public sealed record ProductPrice(Price CurrentPrice, Price? PromotionalPrice);

public sealed record Product(ProductId Id, ProductDescription Description, ProductPrice Price);