using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using ShoppingList.Core.Dto;
using ShoppingList.Core.Repositories;
using ShoppingList.Core.UseCases;

namespace ShoppingList.Infrastructure.OpenTelemetry
{
    public class TracedGetCustomerShoppingListUseCase : GetCustomerShoppingListUseCase
    {
        private static ActivitySource source = new ActivitySource("GetCustomerShoppingListUseCase", "1.0.0");
        public TracedGetCustomerShoppingListUseCase(IShoppingListRepository repository) : base(repository)
        {
        }

        public new async Task<CustomerShoppingListDto> Execute(GetCustomerBasket query, CancellationToken cancellationToken = default)
        {
            using var activity = source.StartActivity();
            activity?.SetTag("customer.id", query.CustomerId.ToString());
            return await base.Execute(query, cancellationToken);
        }
    }
}