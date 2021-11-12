using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using ShoppingList.Core.Model;
using ShoppingList.Core.Repositories;
using static LanguageExt.Prelude;
namespace ShoppingList.Infrastructure.Repositories
{
    internal class GrpcShoppingListRepository : IShoppingListRepository
    {
        private readonly ShoppingListsStorage.ShoppingListsStorage.ShoppingListsStorageClient _client;

        public GrpcShoppingListRepository(ShoppingListsStorage.ShoppingListsStorage.ShoppingListsStorageClient client)
        {
            _client = client;
        }

        public Task AddOrUpdate(CustomerShoppingList customerShopping, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public async Task<Option<CustomerShoppingList>> GetByCustomerId(CustomerId id, CancellationToken cancellationToken = default)
        {
            var result = await _client.GetCustomerShoppingListAsync(new ShoppingListsStorage.GetCustomerShoppingListRequest() {CustomerId = id.Value}, cancellationToken: cancellationToken);
            if (result is null)
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
