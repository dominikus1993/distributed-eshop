using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingList.Core.Dto;
using ShoppingList.Core.Model;
using ShoppingList.Core.Repositories;

namespace ShoppingList.Core.UseCases
{
    internal class GetCustomerShoppingListUseCase
    {
        private IShippingListRepository _repository;

        public GetCustomerShoppingListUseCase(IShippingListRepository repository)
        {
            _repository = repository;
        }

        public async Task<CustomerShoppingList> Execute(GetCustomerBasket query)
        {
            var customerId = new CustomerId(query.CustomerId);
            var basketOpt = await _repository.Get(customerId);
            return basketOpt.IfNone(() => CustomerShoppingList.Empty(customerId));
        }
    }
}
