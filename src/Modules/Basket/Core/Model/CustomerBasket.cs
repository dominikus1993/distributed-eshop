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

    public static CustomerBasket Active(CustomerId id, Products items) => new ActiveBasket(id, items);
    public static CustomerBasket Active(CustomerId id, IEnumerable<Product> items) => new ActiveBasket(id, Products.Create(items));

    public abstract CustomerBasket AddItem(Product item);
    
    public abstract CustomerBasket AddItems(IEnumerable<Product> items);

    public abstract CustomerBasket RemoveItem(Product item);

    public abstract Products Products { get; }
}