using Microsoft.AspNetCore.Mvc;
using ShoppingList.Api.Request;
using ShoppingList.Core.Dto;
using ShoppingList.Core.UseCases;
namespace ShoppingList.Api.Controllers;

[ApiController]
[Route("api/shoppingLists/{customerId}")]
public class ShoppingListController : ControllerBase
{

    private readonly ILogger<ShoppingListController> _logger;

    public ShoppingListController(ILogger<ShoppingListController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetCustomerBasket")]
    public async Task<ActionResult<CustomerShoppingListDto>> Get(int customerId, [FromServices] GetCustomerShoppingListUseCase usecase)
    {
        var basket = await usecase.Execute(new GetCustomerBasket(customerId));
        return Ok(basket);
    }

    [HttpGet("items", Name = "GetCustomerBasket")]
    public async IAsyncEnumerable<ItemDto> GetItems(int customerId, [FromServices] GetCustomerShoppingListUseCase usecase)
    {
        var basket = await usecase.Execute(new GetCustomerBasket(customerId));
        foreach (var item in basket.Items)
        {
            yield return item;
        }
    }

    [HttpPost("items", Name = "AddItemToBasket")]
    public async Task<ActionResult<CustomerShoppingListDto>> AddItem(int customerId, [FromBody] AddItemRequest addItem, [FromServices] AddItemToCustomerShoppingListUseCase usecase)
    {
        await usecase.Execute(new AddItem(customerId,  addItem.ItemId, addItem.ItemQuantity));
        return Ok("Gitówa");
    }

    [HttpDelete("items", Name = "RemoveItemFromBasket")]
    public async Task<ActionResult<CustomerShoppingListDto>> RemoveItem(int customerId, [FromBody] RemoveItemRequest addItem, [FromServices] RemoveItemFromCustomerShoppingList usecase)
    {
        await usecase.Execute(new RemoveItem(customerId, addItem.ItemId, addItem.ItemQuantity));
        return Ok("Gitówa");
    }
}
