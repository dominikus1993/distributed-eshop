using System.Threading.Tasks;
using Refit;
using ShoppingList.Infrastructure.Model;

namespace ShoppingList.Infrastructure.Refit;

internal interface IStorageClient
{
    [Get("/shoppingLists/{customerId}")]
    Task<CustomerShoppingListDto?> GetCustomerShoppingList(int customerId);
}