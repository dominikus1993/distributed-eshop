namespace Basket.Core.Model;

public abstract partial class CustomerBasket
{
    private sealed class ActiveBasket : CustomerBasket
    {
        public ActiveBasket(CustomerId customerId, BasketItems items) : base(customerId)
        {
            BasketItems = items;
        }

        public static ActiveBasket Zero(CustomerId customerId)
        {
            return new ActiveBasket(customerId, BasketItems.Empty);
        }

        public override bool IsEmpty => BasketItems.IsEmpty;

        public override CustomerBasket AddItem(BasketItem item)
        {
            var items = BasketItems.AddItem(item);
            return new ActiveBasket(CustomerId, items);
        }

        public override CustomerBasket AddItems(IReadOnlyCollection<BasketItem> items)
        {
            if (items is { Count: 0} && BasketItems.IsEmpty)
            {
                return new EmptyBasket(CustomerId);
            }
            
            var basketItems = BasketItems.AddItems(items);
            return new ActiveBasket(CustomerId, basketItems);
        }

        public override CustomerBasket RemoveItem(BasketItem item)
        {
            var items = BasketItems.RemoveOrDecreaseItem(item);
            if (items.IsEmpty)
            {
                return new EmptyBasket(CustomerId);
            }

            return new ActiveBasket(CustomerId, items);
        }

        public override BasketItems BasketItems { get; }

        public void Deconstruct(out CustomerId CustomerId, out BasketItems Items)
        {
            CustomerId = this.CustomerId;
            Items = this.BasketItems;
        }
    }   
}