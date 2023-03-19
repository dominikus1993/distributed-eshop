using System.Collections;

namespace Basket.Core.Model;

public sealed class Products : IEnumerable<Product>
{
    private readonly IReadOnlyList<Product> _items;
    public bool IsEmpty => _items.Count == 0;

    private Products(IReadOnlyList<Product> items)
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
            return Enumerable.Empty<T>();
        }

        var array = new T[_items.Count];

        for (int i = 0; i < _items.Count; i++)
        {
            array[i] = map(_items[i]);
        }

        return array;
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