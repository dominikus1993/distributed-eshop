namespace Basket.Core.Model;

public abstract partial class CustomerBasket
{
    private sealed class EmptyBasket : CustomerBasket
    {
        public static EmptyBasket Zero(CustomerId customerId) => new(customerId);

        internal EmptyBasket(CustomerId customerId) : base(customerId)
        {
        }

        public override bool IsEmpty => true;

        public override CustomerBasket AddItem(Product item)
        {
            return ActiveBasket.Zero(CustomerId).AddItem(item);
        }

        public override CustomerBasket AddItems(IEnumerable<Product> items)
        {
            return ActiveBasket.Zero(CustomerId).AddItems(items);
        }

        public override CustomerBasket RemoveItem(Product item)
        {
            return this;
        }

        public override Products Products { get; } = Products.Empty;
    }
}