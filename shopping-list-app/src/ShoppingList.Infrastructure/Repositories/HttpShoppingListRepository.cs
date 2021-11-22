using System.Net;
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

        public async Task Change(CustomerShoppingList customerShopping, CancellationToken cancellationToken = default)
        {
            var request = customerShopping.Items.Select(item => new ItemRequest(item.Id.Value, item.ProductQuantity.Value)).ToArray();
            await _client.ChangeCustomerShoppingList(customerShopping.CustomerId.Value, new ChangeCustomerShoppingListRequest(request));
        }
        public async Task<Option<CustomerShoppingList>> GetByCustomerId(CustomerId id, CancellationToken cancellationToken = default)
        {
            var result = await _client.GetCustomerShoppingList(id.Value);
            if (result.IsSuccessStatusCode)
            {
                var response = result.Content;
                if (response?.Items is null)
                {
                    return None;
                }
                var items = response.Items.Select(x => new Item(new ItemId(x.ItemId), new ItemQuantity(x.ItemQuantity))).ToList();
                return new CustomerShoppingList(new CustomerId(response.CustomerId), items);
            }

            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return None;
            }

            throw new System.Exception($"Response is not successfull, StatusCode: {result.StatusCode}, Reason: {result.ReasonPhrase}");
        }

        public async Task Remove(CustomerShoppingList customerShopping, CancellationToken cancellationToken = default)
        {
            await _client.RemoveCustomerShoppingList(customerShopping.CustomerId.Value);
        }
    }
}
