using System.Collections;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Catalog.Core.Model;

public readonly record struct ProductName(string Name);

public sealed record Tag(string Name);

[CollectionBuilder(typeof(Tags), "Create")]
public sealed record Tags(IReadOnlyCollection<Tag> Value) : IEnumerable<Tag>
{
    public static readonly Tags Empty = new Tags(Array.Empty<Tag>());
    public bool HasElements() => Value is { Count: > 0 };
    
    public static Tags Create(ReadOnlySpan<Tag> value)
    {
        if (value is {Length: 0})
        {
            return Tags.Empty;
        }
        return new Tags(value.ToArray());
    }
    public IEnumerator<Tag> GetEnumerator()
    {
        return Value.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public sealed record ProductDescription(string Description);

public readonly record struct Price(decimal Value)
{
    public static implicit operator Price(decimal value)
    {
        return FromDecimal(value);
    }
    
    public static implicit operator decimal(Price value)
    {
        return ToDecimal(value);
    }

    public static decimal ToDecimal(Price price)
    {
        return price.Value;
    }
    public static Price FromDecimal(decimal value)
    {
        return new Price(value);
    }
}

public sealed record ProductPrice(Price CurrentPrice, Price? PromotionalPrice = null);

public readonly record struct AvailableQuantity(int Value);

public sealed record Product(ProductId Id, ProductName ProductName, ProductDescription ProductDescription, ProductPrice Price,
    AvailableQuantity AvailableQuantity, Tags? Tags);