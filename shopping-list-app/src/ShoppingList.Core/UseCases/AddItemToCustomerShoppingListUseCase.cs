using System;
using System.Threading;
using System.Threading.Tasks;
using ShoppingList.Core.Dto;
using ShoppingList.Core.Model;
using ShoppingList.Core.Repositories;

namespace ShoppingList.Core.UseCases
{
    public class AddItemToCustomerShoppingListUseCase
    {
        private IShippingListRepository _repository;

        public AddItemToCustomerShoppingListUseCase(IShippingListRepository repository)
        {
            _repository = repository;
        }

        public async Task Execute(AddItem addItem, CancellationToken cancellationToken = default)
        {
            if (addItem is null)
            {
                throw new ArgumentNullException(nameof(addItem));
            }

            var customerId = new CustomerId(addItem.CustomerId);
            var basketOpt = await _repository.GetByCustomerId(customerId, cancellationToken);
            var basket = basketOpt.IfNone(() => CustomerShoppingList.Empty(customerId));

            basket.AddItem(new Item(new ItemId(addItem.ItemId), new ItemQuantity(addItem.ItemQuantity)));

            await _repository.AddOrUpdate(basket, cancellationToken);
        }
    }
}
