namespace Basket.Core.Model;

public sealed record BasketItems(IReadOnlyCollection<BasketItem> Items)
{
    public bool IsEmpty => Items.Count == 0;

    public static BasketItems Empty => new(Array.Empty<BasketItem>());
    public BasketItems AddItem(BasketItem item)
    {
        var items = new List<BasketItem>(Items);
        var index = items.IndexOf(item);
        if (index == -1)
        {
            items.Add(item);
        }
        else
        {
            var oldItem = items[index];
            var newItem = oldItem.IncreaseQuantity(item.Quantity);
            items[index] = newItem;   
        }

        return new BasketItems(Items: items);
    }
    
    public BasketItems RemoveOrDecreaseItem(BasketItem item)
    {
        var items = new List<BasketItem>(Items);
        var index = items.IndexOf(item);
        if (index == -1)
        {
            return this;
        }

        var oldItem = items[index];
        var newItem = oldItem.DecreaseQuantity(item.Quantity);
        if (newItem.HasElements)
        {
            items[index] = newItem;  
        }
        else
        {
            items.RemoveAt(index);
        }

        return new BasketItems(Items: items);
    }
}