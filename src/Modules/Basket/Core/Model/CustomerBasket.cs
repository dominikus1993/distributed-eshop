using System.Numerics;

using OneOf;

using StronglyTypedIds;

namespace Basket.Core.Model;

[StronglyTypedId(converters: StronglyTypedIdConverter.SystemTextJson)]
public readonly partial struct CustomerId
{
    
}

public readonly record struct EmptyBasket(CustomerId CustomerId)
{
    public static EmptyBasket Zero(CustomerId customerId) => new(customerId);
}


[StronglyTypedId(backingType: StronglyTypedIdBackingType.Int, converters: StronglyTypedIdConverter.SystemTextJson)]
public readonly partial struct ItemId
{
    
}

public readonly record struct ItemQuantity(uint Value) : IAdditionOperators<ItemQuantity, ItemQuantity, ItemQuantity>, ISubtractionOperators<ItemQuantity, ItemQuantity, ItemQuantity>
{
    public static readonly ItemQuantity Zero = new(0);
    public bool IsZero => Value == 0;
    
    public static ItemQuantity operator +(ItemQuantity left, ItemQuantity right)
    {
        return new ItemQuantity(left.Value + right.Value);
    }

    public static ItemQuantity operator -(ItemQuantity left, ItemQuantity right)
    {
        if (right.Value > left.Value)
        {
            return Zero;
        }

        return new (left.Value - right.Value);
    }
}

public sealed record BasketItem(ItemId ItemId, ItemQuantity Quantity)
{
    public bool Equals(BasketItem? other)
    {
        if (other is null)
        {
            return false;
        }

        return other.ItemId == ItemId;
    }

    public bool HasElements => !Quantity.IsZero;
    
    public override int GetHashCode()
    {
        return ItemId.GetHashCode();
    }

    public BasketItem IncreaseQuantity(ItemQuantity quantity)
    {
        return this with { Quantity = this.Quantity + quantity };
    }

    public BasketItem DecreaseQuantity(ItemQuantity itemQuantity)
    {
        return this with { Quantity = this.Quantity - itemQuantity };
    }
}

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

public sealed record ActiveBasket(CustomerId CustomerId, BasketItems Items)
{
    public bool HasItems => !Items.IsEmpty;
    
    public static ActiveBasket Zero(CustomerId customerId)
    {
        return new ActiveBasket(customerId, BasketItems.Empty);
    }
    
    public ActiveBasket AddItem(BasketItem item)
    {
        var items = Items.AddItem(item);
        return this with { Items = items };
    }
    
    public ActiveBasket RemoveOrDecreaseItem(BasketItem item)
    {
        var items = Items.RemoveOrDecreaseItem(item);
        return this with { Items = items };
    }
}

[GenerateOneOf]
public sealed partial class CustomerBasket: OneOfBase<EmptyBasket, ActiveBasket>
{
    public bool IsEmpty => IsT0;
    public CustomerId CustomerId => Match(b => b.CustomerId, basket => basket.CustomerId);
    
    public static CustomerBasket Empty(CustomerId id) => EmptyBasket.Zero(id);

    public CustomerBasket AddItem(BasketItem item)
    {
        return Match(b => ActiveBasket.Zero(b.CustomerId).AddItem(item), b => b.AddItem(item));
    }
    
    public CustomerBasket RemoveItem(BasketItem item)
    {
        return Match<CustomerBasket>(b => b, b =>
        {
            var newBasket = b.RemoveOrDecreaseItem(item);
            if (newBasket.HasItems)
            {
                return newBasket;
            }

            return EmptyBasket.Zero(b.CustomerId);
        });
    }

    public BasketItems GetItems() => Match(_ => BasketItems.Empty, b => b.Items);
}