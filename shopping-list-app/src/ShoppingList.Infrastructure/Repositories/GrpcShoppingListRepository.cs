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
    public class GrpcShoppingListRepository : IShoppingListRepository
    {
        private readonly ShoppingListsStorage.ShoppingListsStorage.ShoppingListsStorageClient _client;

        public GrpcShoppingListRepository(ShoppingListsStorage.ShoppingListsStorage.ShoppingListsStorageClient client)
        {
            _client = client;
        }

        public async Task Change(CustomerShoppingList customerShopping, CancellationToken cancellationToken = default)
        {
            var items = customerShopping.Items.Select(item => new ShoppingListsStorage.CustomerShoppingList.Types.Item { ItemId = item.Id.Value, ItemQuantity = item.ProductQuantity.Value });
            var shoppingList = new ShoppingListsStorage.CustomerShoppingList() { CustomerId = customerShopping.CustomerId.Value };
            shoppingList.Items.AddRange(items);
            await _client.ChangeCustomerShoppingListAsync(new ShoppingListsStorage.ChangeCustomerShoppingListRequest { CustomerId = customerShopping.CustomerId.Value, ShoppingList = shoppingList});
        }

        public async Task<Option<CustomerShoppingList>> GetByCustomerId(CustomerId id, CancellationToken cancellationToken = default)
        {
            var result = await _client.GetCustomerShoppingListAsync(new ShoppingListsStorage.GetCustomerShoppingListRequest() { CustomerId = id.Value }, cancellationToken: cancellationToken);
            if (result is null)
            {
                return None;
            }
            var items = result.Items.Select(x => new Item(new ItemId(x.ItemId), new ItemQuantity(x.ItemQuantity))).ToList();
            return new CustomerShoppingList(new CustomerId(result.CustomerId), items);
        }

        public async Task Remove(CustomerShoppingList customerShopping, CancellationToken cancellationToken = default)
        {
            await _client.RemoveCustomerShoppingListAsync(new ShoppingListsStorage.RemoveCustomerShoppingListRequest() { CustomerId = customerShopping.CustomerId.Value }, cancellationToken: cancellationToken);
        }
    }
}