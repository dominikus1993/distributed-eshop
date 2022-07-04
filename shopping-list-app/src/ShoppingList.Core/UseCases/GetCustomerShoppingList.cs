using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ShoppingList.Core.Dto;
using ShoppingList.Core.Model;
using ShoppingList.Core.Repositories;

namespace ShoppingList.Core.UseCases;

public class GetCustomerShoppingListUseCase
{
    private static readonly ActivitySource Source = new ActivitySource(nameof(GetCustomerShoppingListUseCase), "1.0.0");
    private IShoppingListRepository _repository;

    public GetCustomerShoppingListUseCase(IShoppingListRepository repository)
    {
        _repository = repository;
    }

    public async Task<CustomerShoppingListDto> Execute(GetCustomerBasket query, CancellationToken cancellationToken = default)
    {
        using var activity = Source.StartActivity("get.customer.shopping.list");
        activity?.SetTag("customer.id", query.CustomerId.ToString());
        var customerId = new CustomerId(query.CustomerId);
        var basketOpt = await _repository.GetByCustomerId(customerId, cancellationToken);
        var basket = basketOpt.IfNone(() => CustomerShoppingList.Empty(customerId));
        return new CustomerShoppingListDto(basket);
    }
}
