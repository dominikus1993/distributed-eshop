using System.Collections.Generic;
using System.Linq;
using ShoppingList.Core.Model;

namespace ShoppingList.Core.Dto;

public record RemoveCustomerShoppingList(int CustomerId);
public record AddItem(int CustomerId, int ItemId, int ItemQuantity);
public record RemoveItem(int CustomerId, int ItemId, int ItemQuantity);
public record GetCustomerBasket(int CustomerId);
public record ItemDto(int ItemId, int ItemQuantity)
{
    public ItemDto(Item item) : this(item.Id.Value, item.ProductQuantity.Value)
    {

    }
}
public record CustomerShoppingListDto(int CustomerId, IEnumerable<ItemDto> Items)
{
    public CustomerShoppingListDto(CustomerShoppingList basket) : this(basket.CustomerId.Value, basket.Items.Select(it => new ItemDto(it)).ToList())
    {

    }
}

