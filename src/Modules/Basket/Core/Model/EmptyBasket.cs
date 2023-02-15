namespace Basket.Core.Model;

public abstract partial class CustomerBasket
{
    public sealed class EmptyBasket : CustomerBasket
    {
        public static EmptyBasket Zero(CustomerId customerId) => new(customerId);

        internal EmptyBasket(CustomerId customerId) : base(customerId)
        {
        }

        public override bool IsEmpty => true;

        public override CustomerBasket AddItem(BasketItem item)
        {
            return ActiveBasket.Zero(CustomerId).AddItem(item);
        }

        public override CustomerBasket RemoveItem(BasketItem item)
        {
            return this;
        }

        public override BasketItems BasketItems { get; } = BasketItems.Empty;
    }
}