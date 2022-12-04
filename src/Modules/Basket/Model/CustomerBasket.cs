using OneOf;

using StronglyTypedIds;

namespace Basket.Model;

[StronglyTypedId]
public readonly partial struct CustomerId
{
    
}

public readonly record struct EmptyBasket(CustomerId CustomerId);

[StronglyTypedId]
public readonly partial struct ItemId
{
    
}

public readonly record struct ItemQuantity(uint Value);

public sealed record BasketItem(ItemId ItemId, ItemQuantity Quantity);

public sealed class ActiveBasket
{
    
    public required CustomerId CustomerId { get; init; }
    public required IReadOnlyCollection<BasketItem> Items { get; init; }

    private ActiveBasket()
    {
        
    }

    public static ActiveBasket Zero(CustomerId customerId)
    {
        return new ActiveBasket { CustomerId = customerId, Items = Array.Empty<BasketItem>() };
    } 
}



[GenerateOneOf]
public sealed partial class CustomerBasket: OneOfBase<EmptyBasket, ActiveBasket>
{
    public IReadOnlyCollection<BasketItem> Items => base.IsT0 ? Array.Empty<BasketItem>() : base.AsT1.Items;
    
    public static CustomerBasket Empty(CustomerId id) => new EmptyBasket(id);
}