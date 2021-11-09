using LanguageExt;
using ShoppingList.Core.Model;

namespace ShoppingList.Core.Repositories
{
    public interface IShippingListRepository
    {
        Task AddOrUpdate(CustomerShoppingList customerShopping);
        Task Remove(CustomerShoppingList customerShopping);
        Task<Option<CustomerShoppingList>> Get(CustomerId id);
    }
}
