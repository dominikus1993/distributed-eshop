using System.Collections;
using System.Runtime.CompilerServices;

using Basket.Infrastructure.Extensions;

namespace Basket.Core.Model;

[CollectionBuilder(typeof(Products), "Create")]
public sealed class Products : IEnumerable<Product>
{
    private readonly IReadOnlyList<Product> _items;
    public bool IsEmpty => _items.Count == 0;

    private Products(IReadOnlyList<Product> items)
    {
        _items = items;
    }
    
    public static readonly Products Empty = new([]);
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
        var aggreated = this;
        foreach (var item in items)
        {
            aggreated.AddItem(item);
        }
        return aggreated;
    }
    
    public static Products Create(ReadOnlySpan<Product> value)
    {
        if (value.IsEmpty)
        {
            return Empty;
        }
        return new Products(value.ToArray());
    }
    
    public static Products Create(IEnumerable<Product> value)
    {
        var products = value.ToArray();
        if (products is {Length: 0 })
        {
            return Empty;
        }
        
        return new Products(products);
    }


    public IReadOnlyList<T> MapItems<T>(Func<Product, T> map)
    {
        switch (_items)
        {
            case null or {Count: 0}:
                return Array.Empty<T>();
            case [var element]:
                return new[] { map(element) };
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