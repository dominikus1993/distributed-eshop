namespace ShoppingList.Core.Dto
{
    public record AddItem(int CustomerId, int ItemId, int ItemQuantity);
    public record RemoveItem(int CustomerId, int ItemId, int ItemQuantity);
    public record GetCustomerBasket(int CustomerId);
    public record ItemDto(int ItemId, int ItemQuantity);
    public record CustomerShoppingListDto(int CustomerId, List<ItemDto> Items);
}
