using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using ShoppingList.Core.Model;
using ShoppingList.Core.Repositories;
using ShoppingList.Infrastructure.Refit;
using static LanguageExt.Prelude;
namespace ShoppingList.Infrastructure.Repositories
{
    internal class HttpShoppingListRepository : IShoppingListRepository
    {
        private readonly IStorageClient _client;

        public HttpShoppingListRepository(IStorageClient client)
        {
            _client = client;
        }

        public Task AddOrUpdate(CustomerShoppingList customerShopping, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
        public async Task<Option<CustomerShoppingList>> GetByCustomerId(CustomerId id, CancellationToken cancellationToken = default)
        {
            var result = await _client.GetCustomerShoppingList(id.Value);
            if (result?.Items is null)
            {
                return None;
            }
            var items = result.Items.Select(x => new Item(new ItemId(x.ItemId), new ItemQuantity(x.ItemQuantity))).ToList();
            return new CustomerShoppingList(new CustomerId(result.CustomerId), items);
        }

        public Task Remove(CustomerShoppingList customerShopping, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
