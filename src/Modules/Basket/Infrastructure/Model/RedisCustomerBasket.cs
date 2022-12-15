using System.Diagnostics.CodeAnalysis;

using Basket.Core.Model;

namespace Basket.Infrastructure.Model;


internal sealed class RedisBasketItem
{
    public int ItemId { get; set; }
    public uint Quantity { get; set; }

    public RedisBasketItem()
    {
        
    }

    public RedisBasketItem(BasketItem item)
    {
        ItemId = item.ItemId.Value;
        Quantity = item.Quantity.Value;
    }
}


internal sealed class RedisCustomerBasket
{
    public Guid CustomerId { get; set; }
    public IReadOnlyCollection<RedisBasketItem> Items { get; set; }
    
    public RedisCustomerBasket()
    {
        
    }
    
    public RedisCustomerBasket(CustomerBasket basket)
    {
        CustomerId = basket.CustomerId.Value;
        Items = basket.BasketItems.Items.Select(item => new RedisBasketItem(item)).ToArray();
    }
}