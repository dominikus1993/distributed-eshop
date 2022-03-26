namespace ShoppingList.Api.Request;

public record AddItemRequest(int ItemId, int ItemQuantity);
public record RemoveItemRequest(int ItemId, int ItemQuantity);
