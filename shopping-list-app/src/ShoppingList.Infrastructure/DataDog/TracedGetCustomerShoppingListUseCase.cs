using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Datadog.Trace;
using ShoppingList.Core.Dto;
using ShoppingList.Core.Repositories;
using ShoppingList.Core.UseCases;

namespace ShoppingList.Infrastructure.DataDog
{
    public class TracedGetCustomerShoppingListUseCase : GetCustomerShoppingListUseCase
    {
        public TracedGetCustomerShoppingListUseCase(IShoppingListRepository repository) : base(repository)
        {
        }

        public new async Task<CustomerShoppingListDto> Execute(GetCustomerBasket query, CancellationToken cancellationToken = default)
        {
            using var scope = Tracer.Instance.StartActive("get.customer.shoppinglist");
            var span = scope.Span;
            span.SetTag("customer.id", query.CustomerId.ToString());
            return await base.Execute(query, cancellationToken);
        }
    }
}