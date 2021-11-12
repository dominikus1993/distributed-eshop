using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingList.Api.Request;
using ShoppingList.Core.Dto;
using ShoppingList.Core.UseCases;

namespace ShoppingList.Api.Modules
{
    public static class CustomerShoppingList
    {
        public static async Task<IResult> GetCustomerShoppingList(int customerId, GetCustomerShoppingListUseCase usecase)
        {
            var basket = await usecase.Execute(new GetCustomerBasket(customerId));
            return Results.Ok(basket);
        }

        public static async IAsyncEnumerable<ItemDto> GetCustomerShoppingListItems(int customerId, GetCustomerShoppingListItemsUseCase usecase)
        {
            await foreach (var item in usecase.Execute(new GetCustomerBasket(customerId)))
            {
                yield return item;
            }
        }

        public static async Task<IResult> AddItemToCustomerShoppingList(int customerId, AddItemRequest addItem, AddItemToCustomerShoppingListUseCase usecase)
        {
            if (addItem is null)
            {
                return Results.BadRequest("Item can't be null");
            }
            await usecase.Execute(new AddItem(customerId, addItem.ItemId, addItem.ItemQuantity));
            return Results.Accepted();
        }

        public static async Task<IResult> RemoveItemFromCustomerShoppingList(int customerId, RemoveItemRequest removeItem, RemoveItemFromCustomerShoppingList usecase)
        {
            await usecase.Execute(new RemoveItem(customerId, removeItem.ItemId, removeItem.ItemQuantity));
            return Results.NoContent();
        }

        public static async Task<IResult> RemoveCustomerShoppingList(int customerId,  RemoveCustomerShoppingListUseCase usecase)
        {
            await usecase.Execute(new RemoveCustomerShoppingList(customerId));
            return Results.NoContent();
        }
    }
}