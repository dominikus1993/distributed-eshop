namespace Basket.Core.Model;

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