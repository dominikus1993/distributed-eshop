namespace Basket.Core.Model;

public abstract partial class CustomerBasket
{
    private sealed class ActiveBasket : CustomerBasket
    {
        public ActiveBasket(CustomerId customerId, Products items) : base(customerId)
        {
            Products = items;
        }

        public static ActiveBasket Zero(CustomerId customerId)
        {
            return new ActiveBasket(customerId, Products.Empty);
        }

        public override bool IsEmpty => Products.IsEmpty;

        public override CustomerBasket AddItem(Product item)
        {
            var items = Products.AddItem(item);
            return new ActiveBasket(CustomerId, items);
        }

        public override CustomerBasket AddItems(IReadOnlyCollection<Product> items)
        {
            if (items is { Count: 0} && Products.IsEmpty)
            {
                return new EmptyBasket(CustomerId);
            }
            
            var basketItems = Products.AddItems(items);
            return new ActiveBasket(CustomerId, basketItems);
        }

        public override CustomerBasket RemoveItem(Product item)
        {
            var items = Products.RemoveOrDecreaseItem(item);
            if (items.IsEmpty)
            {
                return new EmptyBasket(CustomerId);
            }

            return new ActiveBasket(CustomerId, items);
        }

        public override Products Products { get; }

        public void Deconstruct(out CustomerId CustomerId, out Products Items)
        {
            CustomerId = this.CustomerId;
            Items = this.Products;
        }
    }   
}