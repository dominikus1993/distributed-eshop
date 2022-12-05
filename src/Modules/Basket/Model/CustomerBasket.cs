using System.Numerics;

using OneOf;

using StronglyTypedIds;

namespace Basket.Model;

[StronglyTypedId(converters: StronglyTypedIdConverter.SystemTextJson)]
public readonly partial struct CustomerId
{
    
}

public readonly record struct EmptyBasket(CustomerId CustomerId);


[StronglyTypedId(backingType: StronglyTypedIdBackingType.Int, converters: StronglyTypedIdConverter.SystemTextJson)]
public readonly partial struct ItemId
{
    
}

public readonly record struct ItemQuantity(uint Value) : IAdditionOperators<ItemQuantity, ItemQuantity, ItemQuantity>
{
    public static readonly ItemQuantity Zero = new(0);
    public static ItemQuantity operator +(ItemQuantity left, ItemQuantity right)
    {
        return new ItemQuantity(left.Value + right.Value);
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
    
    public override int GetHashCode()
    {
        return ItemId.GetHashCode();
    }

    public BasketItem IncreaseQuantity(ItemQuantity quantity)
    {
        return this with { Quantity = this.Quantity + quantity };
    }
}

public sealed record BasketItems(IReadOnlyCollection<BasketItem> Items)
{
    public bool IsEmpty => Items.Count == 0;

    public static BasketItems Empty() => new(Array.Empty<BasketItem>());
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
}

public sealed record ActiveBasket(CustomerId CustomerId, BasketItems Items)
{
    public static ActiveBasket Zero(CustomerId customerId)
    {
        return new ActiveBasket(customerId, BasketItems.Empty());
    }
    
    public ActiveBasket AddItem(BasketItem item)
    {
        var items = Items.AddItem(item);
        return this with { Items = items };
    }
}

[GenerateOneOf]
public sealed partial class CustomerBasket: OneOfBase<EmptyBasket, ActiveBasket>
{
    public BasketItems Items => base.IsT0 ? BasketItems.Empty(): base.AsT1.Items;
    
    public static CustomerBasket Empty(CustomerId id) => new EmptyBasket(id);

    public CustomerBasket AddItem(BasketItem item)
    {
        return Match(b => ActiveBasket.Zero(b.CustomerId).AddItem(item), b => b.AddItem(item));
    }
}