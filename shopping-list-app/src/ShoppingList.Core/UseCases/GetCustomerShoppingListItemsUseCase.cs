using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ShoppingList.Core.Dto;
using ShoppingList.Core.Model;
using ShoppingList.Core.Repositories;

namespace ShoppingList.Core.UseCases
{

    public class GetCustomerShoppingListItemsUseCase
    {
        private IShippingListRepository _repository;

        public GetCustomerShoppingListItemsUseCase(IShippingListRepository repository)
        {
            _repository = repository;
        }

        public async IAsyncEnumerable<ItemDto> Execute(GetCustomerBasket query, CancellationToken cancellationToken = default)
        {
            var customerId = new CustomerId(query.CustomerId);
            var basketOpt = await _repository.GetByCustomerId(customerId, cancellationToken);

            foreach (var item in basketOpt.Where(b => b.Items.Count > 0).Map(b => b.Items).IfNone(() => new List<Item>(0)))
            {
                yield return new ItemDto(item);
            }
           
        }
    }
}
