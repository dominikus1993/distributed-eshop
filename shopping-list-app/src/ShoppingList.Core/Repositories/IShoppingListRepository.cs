using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using ShoppingList.Core.Model;

namespace ShoppingList.Core.Repositories
{
    public interface IShoppingListRepository
    {
        Task AddOrUpdate(CustomerShoppingList customerShopping, CancellationToken cancellationToken = default);
        Task Remove(CustomerShoppingList customerShopping, CancellationToken cancellationToken = default);
        Task<Option<CustomerShoppingList>> GetByCustomerId(CustomerId id, CancellationToken cancellationToken = default);
    }
}
