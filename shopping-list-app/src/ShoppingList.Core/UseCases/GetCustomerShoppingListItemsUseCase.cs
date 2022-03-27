using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ShoppingList.Core.Dto;
using ShoppingList.Core.Model;
using ShoppingList.Core.Repositories;

namespace ShoppingList.Core.UseCases;

public class GetCustomerShoppingListItemsUseCase
{
    private IShoppingListRepository _repository;

    public GetCustomerShoppingListItemsUseCase(IShoppingListRepository repository)
    {
        _repository = repository;
    }

    public async IAsyncEnumerable<ItemDto> Execute(GetCustomerBasket query, [EnumeratorCancellation]CancellationToken cancellationToken = default)
    {
        var customerId = new CustomerId(query.CustomerId);
        var basketOpt = await _repository.GetByCustomerId(customerId, cancellationToken);
        var items = basketOpt.Where(b => b.Items.Count > 0).Map(b => b.Items).IfNone(() => new List<Item>(0));

        if (items.Count == 0)
        {
            yield break;
        }

        foreach (var item in items)
        {
            yield return new ItemDto(item);
        }
    }
}
