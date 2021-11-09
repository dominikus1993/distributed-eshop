using System.Threading.Tasks;
using ShoppingList.Core.Dto;
using ShoppingList.Core.Model;
using ShoppingList.Core.Repositories;

namespace ShoppingList.Core.UseCases
{
    public class RemoveItemFromCustomerShoppingList
    {
        private IShippingListRepository _repository;

        public RemoveItemFromCustomerShoppingList(IShippingListRepository repository)
        {
            _repository = repository;
        }

        public async Task Execute(AddItem addItem)
        {
            var customerId = new CustomerId(addItem.CustomerId);
            var basketOpt = await _repository.Get(customerId);
            var basket = basketOpt.IfNone(() => CustomerShoppingList.Empty(customerId));
            await _repository.Remove(basket);
        }
    }

}
