using System.Threading;
using System.Threading.Tasks;
using ShoppingList.Core.Dto;
using ShoppingList.Core.Model;
using ShoppingList.Core.Repositories;

namespace ShoppingList.Core.UseCases;

public sealed class RemoveCustomerShoppingListUseCase
{
    private IShoppingListRepository _repository;

    public RemoveCustomerShoppingListUseCase(IShoppingListRepository repository)
    {
        _repository = repository;
    }

    public async Task Execute(RemoveCustomerShoppingList rm, CancellationToken cancellationToken = default)
    {
        var customerId = new CustomerId(rm.CustomerId);
        var basketOpt = await _repository.GetByCustomerId(customerId, cancellationToken);           
        await basketOpt.IfSomeAsync(async basket => await _repository.Remove(basket));
    }
}
