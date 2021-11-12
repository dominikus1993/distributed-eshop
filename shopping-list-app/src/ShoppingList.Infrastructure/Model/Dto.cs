using System.Collections.Generic;

namespace ShoppingList.Infrastructure.Model
{
    public class ItemDto
    {
        public int ItemId { get; set; }
        public int ItemQuantity { get; set; }                 
    }

    public class CustomerShoppingListDto
    {
        public int CustomerId { get; set; }
        public List<ItemDto>? Items { get; set; }       
    }
}