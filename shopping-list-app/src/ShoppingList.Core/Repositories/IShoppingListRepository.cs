using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using ShoppingList.Core.Model;

namespace ShoppingList.Core.Repositories
{
    public interface IShoppingListWriter
    {
        Task Change(CustomerShoppingList customerShopping, CancellationToken cancellationToken = default);
        Task Remove(CustomerShoppingList customerShopping, CancellationToken cancellationToken = default);
    }

    public interface IShoppingListReader
    {
        Task<Option<CustomerShoppingList>> GetByCustomerId(CustomerId id, CancellationToken cancellationToken = default);
    }
    
    public interface IShoppingListRepository : IShoppingListWriter, IShoppingListReader
    {
    }
}
