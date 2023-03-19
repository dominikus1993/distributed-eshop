using System.Collections;

namespace Basket.Core.Model;

public sealed class Products : IEnumerable<Product>
{
    private readonly IReadOnlyCollection<Product> _items;
    public bool IsEmpty => _items.Count == 0;

    private Products(IReadOnlyCollection<Product> items)
    {
        _items = items;
    }
    
    public static Products Empty => new(Array.Empty<Product>());
    public Products AddItem(Product item)
    {
        var items = new List<Product>(_items);
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

        return new Products(items);
    }
    
    public Products AddItems(IEnumerable<Product> items)
    {
        return items.Aggregate(this, (basketItems, item) => basketItems.AddItem(item));
    }

    public IEnumerable<T> MapItems<T>(Func<Product, T> map)
    {
        if (_items is {Count: 0})
        {
            yield break;
        }

        foreach (Product basketItem in _items)
        {
            yield return map(basketItem);
        }
    }
    
    public Products RemoveOrDecreaseItem(Product item)
    {
        var items = new List<Product>(_items);
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

        return new Products(items);
    }

    public IEnumerator<Product> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}