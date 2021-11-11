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
    internal class HttpShoppingListRepository : IShoppingListRepository
    {
        private readonly HttpClient _client;

        public HttpShoppingListRepository(HttpClient client)
        {
            _client = client;
        }

        public Task AddOrUpdate(CustomerShoppingList customerShopping, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public async Task<Option<CustomerShoppingList>> GetByCustomerId(CustomerId id, CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            return None;
        }

        public Task Remove(CustomerShoppingList customerShopping, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
