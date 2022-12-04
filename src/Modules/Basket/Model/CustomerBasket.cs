using OneOf;

namespace Basket.Model;

public readonly record struct CustomerId(Guid Value);

public readonly record struct EmptyBasket(CustomerId CustomerId);

public readonly record struct ItemId(Guid Id);

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
    
}