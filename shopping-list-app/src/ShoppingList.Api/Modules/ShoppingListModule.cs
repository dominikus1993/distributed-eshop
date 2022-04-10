using System.Diagnostics.Metrics;

using Microsoft.AspNetCore.Mvc;
using ShoppingList.Api.Request;
using ShoppingList.Core.Dto;
using ShoppingList.Core.UseCases;

namespace ShoppingList.Api.Modules
{
    public static class CustomerShoppingList
    {
        private static Meter _meter = new Meter(nameof(CustomerShoppingList), "1.0.0");
        private static Counter<int> _counter = _meter.CreateCounter<int>("shopinglists_requests");
        public static async Task<IResult> GetCustomerShoppingList(int customerId, GetCustomerShoppingListUseCase usecase, CancellationToken cancellationToken)
        {
            _counter.Add(1);
            var basket = await usecase.Execute(new GetCustomerBasket(customerId));
            return Results.Ok(basket);
        }

        public static IAsyncEnumerable<ItemDto> GetCustomerShoppingListItems(int customerId, GetCustomerShoppingListItemsUseCase usecase, CancellationToken cancellationToken)
        {
            return usecase.Execute(new GetCustomerBasket(customerId), cancellationToken);
        }

        public static async Task<IResult> AddItemToCustomerShoppingList(int customerId, AddItemRequest addItem, AddItemToCustomerShoppingListUseCase usecase, CancellationToken cancellationToken)
        {
            if (addItem is null)
            {
                return Results.BadRequest("Item can't be null");
            }
            await usecase.Execute(new AddItem(customerId, addItem.ItemId, addItem.ItemQuantity), cancellationToken);
            return Results.Accepted();
        }

        public static async Task<IResult> RemoveItemFromCustomerShoppingList(int customerId, [FromBody]RemoveItemRequest removeItem, [FromServices]RemoveItemFromCustomerShoppingList usecase, CancellationToken cancellationToken)
        {
            await usecase.Execute(new RemoveItem(customerId, removeItem.ItemId, removeItem.ItemQuantity), cancellationToken);
            return Results.NoContent();
        }

        public static async Task<IResult> RemoveCustomerShoppingList(int customerId,  RemoveCustomerShoppingListUseCase usecase, CancellationToken cancellationToken)
        {
            await usecase.Execute(new RemoveCustomerShoppingList(customerId), cancellationToken);
            return Results.NoContent();
        }
    }
}