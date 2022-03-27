using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShoppingList.Core.Dto;
using ShoppingList.Core.Logging;
using ShoppingList.Core.Model;
using ShoppingList.Core.Repositories;

namespace ShoppingList.Core.UseCases;

public class AddItemToCustomerShoppingListUseCase
{
    private IShoppingListRepository _repository;
    private ILogger<AddItemToCustomerShoppingListUseCase> _logger;

    public AddItemToCustomerShoppingListUseCase(IShoppingListRepository repository, ILogger<AddItemToCustomerShoppingListUseCase> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Execute(AddItem addItem, CancellationToken cancellationToken = default)
    {
        if (addItem is null)
        {
            throw new ArgumentNullException(nameof(addItem));
        }

        _logger.LogAddItem(addItem);

        var customerId = new CustomerId(addItem.CustomerId);
        var basketOpt = await _repository.GetByCustomerId(customerId, cancellationToken);
        var basket = basketOpt.IfNone(() => CustomerShoppingList.Empty(customerId));

        basket.AddItem(new Item(new ItemId(addItem.ItemId), new ItemQuantity(addItem.ItemQuantity)));

        await _repository.Change(basket, cancellationToken);
    }
}
