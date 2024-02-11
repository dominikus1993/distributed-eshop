using System.Numerics;

namespace Warehouse.Core.Model;

public readonly record struct ItemQuantity(int Value) : IAdditionOperators<ItemQuantity, ItemQuantity, ItemQuantity>, ISubtractionOperators<ItemQuantity, ItemQuantity, ItemQuantity>
{
    public static readonly ItemQuantity Zero = new(0);
    public static ItemQuantity operator +(ItemQuantity left, ItemQuantity right)
    {
        return new ItemQuantity(left.Value + right.Value);
    }

    public static ItemQuantity operator -(ItemQuantity left, ItemQuantity right)
    {
        var res = left.Value - right.Value;
        return res < 0 ? Zero : new ItemQuantity(res);
    }
}

public sealed record WarehouseItem(ProductId ProductId, ItemQuantity Quantity)
{
}