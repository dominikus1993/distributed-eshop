using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingList.Core.Model;

namespace ShoppingList.Infrastructure.Model
{
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
}