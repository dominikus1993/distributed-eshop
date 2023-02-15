using System.Collections;

using OneOf;

namespace Basket.Core.Model;

public abstract partial class CustomerBasket
{
    public CustomerId CustomerId { get; }

    private CustomerBasket(CustomerId customerId)
    {
        CustomerId = customerId;
    }
    
    public abstract bool IsEmpty { get; }
    
    public static CustomerBasket Empty(CustomerId id) => EmptyBasket.Zero(id);

    public static CustomerBasket Active(CustomerId id, BasketItems items) => new ActiveBasket(id, items);

    public abstract CustomerBasket AddItem(BasketItem item);

    public abstract CustomerBasket RemoveItem(BasketItem item);

    public abstract BasketItems BasketItems { get; }
}