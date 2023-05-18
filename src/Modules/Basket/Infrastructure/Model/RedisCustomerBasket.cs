using System.Diagnostics.CodeAnalysis;

using Basket.Core.Model;

using MemoryPack;

namespace Basket.Infrastructure.Model;

[MemoryPackable]
internal sealed partial class RedisBasketItem
{
    public Guid ItemId { get; set; }
    public uint Quantity { get; set; }

    [MemoryPackConstructor]
    public RedisBasketItem()
    {
        
    }

    public RedisBasketItem(Product item)
    {
        ItemId = item.ItemId.Value;
        Quantity = item.Quantity.Value;
    }
}


[MemoryPackable]
internal sealed partial class RedisCustomerBasket
{
    public Guid CustomerId { get; set; }
    public IReadOnlyCollection<RedisBasketItem> Items { get; set; } = null!;
    public long ModifiedAtTimestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    
    [MemoryPackConstructor]
    public RedisCustomerBasket()
    {
        
    }
    
    public RedisCustomerBasket(CustomerBasket basket)
    {
        CustomerId = basket.CustomerId.Value;
        Items = basket.Products.MapItems(static item => new RedisBasketItem(item));
    }
}