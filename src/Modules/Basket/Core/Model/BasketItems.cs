using System.Collections;

namespace Basket.Core.Model;

public sealed class BasketItems : IEnumerable<BasketItem>
{
    private readonly IReadOnlyCollection<BasketItem> _items;
    public bool IsEmpty => _items.Count == 0;

    private BasketItems(IReadOnlyCollection<BasketItem> items)
    {
        _items = items;
    }
    
    public static BasketItems Empty => new(Array.Empty<BasketItem>());
    public BasketItems AddItem(BasketItem item)
    {
        var items = new List<BasketItem>(_items);
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

        return new BasketItems(items);
    }
    
    public BasketItems AddItems(IEnumerable<BasketItem> items)
    {
        return items.Aggregate(this, (basketItems, item) => basketItems.AddItem(item));
    }

    public IEnumerable<T> MapItems<T>(Func<BasketItem, T> map)
    {
        if (_items is {Count: 0})
        {
            yield break;
        }

        foreach (BasketItem basketItem in _items)
        {
            yield return map(basketItem);
        }
    }
    
    public BasketItems RemoveOrDecreaseItem(BasketItem item)
    {
        var items = new List<BasketItem>(_items);
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

        return new BasketItems(items);
    }

    public IEnumerator<BasketItem> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}