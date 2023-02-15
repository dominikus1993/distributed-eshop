using System.Numerics;

namespace Basket.Core.Model;

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

    public static ItemQuantity Add(ItemQuantity left, ItemQuantity right)
    {
        return left + right;
    }

    public static ItemQuantity Subtract(ItemQuantity left, ItemQuantity right)
    {
        return left - right;
    }
}