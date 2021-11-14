using System.Threading.Tasks;
using Refit;
using ShoppingList.Infrastructure.Model;

namespace ShoppingList.Infrastructure.Refit;

internal record ItemRequest(int ItemId, int ItemQuantity);
internal record ChangeCustomerShoppingListRequest(ItemRequest[] Items);
internal interface IStorageClient
{
    [Get("/shoppingLists/{customerId}")]
    Task<ApiResponse<CustomerShoppingListDto?>> GetCustomerShoppingList(int customerId);
    
    [Post("/shoppingLists/{customerId}")]
    Task<CustomerShoppingListDto?> ChangeCustomerShoppingList(int customerId, [Body]ChangeCustomerShoppingListRequest request);
    
    [Delete("/shoppingLists/{customerId}")]
    Task RemoveCustomerShoppingList(int customerId);
}