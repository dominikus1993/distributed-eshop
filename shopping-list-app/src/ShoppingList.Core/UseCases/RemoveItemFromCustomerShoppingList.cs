using System;
using System.Threading;
using System.Threading.Tasks;

using LanguageExt.UnsafeValueAccess;

using ShoppingList.Core.Dto;
using ShoppingList.Core.Model;
using ShoppingList.Core.Repositories;

namespace ShoppingList.Core.UseCases;

public class RemoveItemFromCustomerShoppingList
{
    private IShoppingListRepository _repository;

    public RemoveItemFromCustomerShoppingList(IShoppingListRepository repository)
    {
        _repository = repository;
    }

    public async Task Execute(RemoveItem removeItem, CancellationToken cancellationToken = default)
    {
        if (removeItem is null)
        {
            throw new ArgumentNullException(nameof(removeItem));
        }

        var customerId = new CustomerId(removeItem.CustomerId);
        var basketOpt = await _repository.GetByCustomerId(customerId, cancellationToken);

        if (basketOpt.IsNone)
        {
            return;
        }

        var basket = basketOpt.ValueUnsafe();

        await basketOpt.IfSomeAsync(async basket =>
        {
            basket.RemoveItem(new Item(new ItemId(removeItem.ItemId), new ItemQuantity(removeItem.ItemQuantity)));

            await _repository.Change(basket, cancellationToken);
        });
    }
}

