using System.Diagnostics.CodeAnalysis;

using Basket.Core.Model;

namespace Basket.Core.Dtos;

public sealed class BasketItemDto
{
    public required Guid ItemId { get; init; }
    public required uint Quantity { get; init; }

    public BasketItemDto()
    {
        
    }
    
    [SetsRequiredMembers]
    public BasketItemDto(BasketItem item)
    {
        ItemId = item.ItemId.Value;
        Quantity = item.Quantity.Value;
    }
}


public sealed class CustomerBasketDto
{
    public required CustomerId CustomerId { get; init; }
    public required IReadOnlyCollection<BasketItemDto> Items { get; init; }
    
    public CustomerBasketDto()
    {
        
    }
    
    [SetsRequiredMembers]
    public CustomerBasketDto(CustomerBasket basket)
    {
        CustomerId = basket.CustomerId;
        Items = basket.BasketItems.MapItems(item => new BasketItemDto(item)).ToArray();
    }
}