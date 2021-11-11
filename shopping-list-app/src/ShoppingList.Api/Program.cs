using ShoppingList.Api.Modules;
using ShoppingList.Core.UseCases;
using ShoppingList.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Core
builder.Services.AddTransient<AddItemToCustomerShoppingListUseCase>();
builder.Services.AddTransient<GetCustomerShoppingListUseCase>();
builder.Services.AddTransient<RemoveItemFromCustomerShoppingList>();
builder.Services.AddTransient<GetCustomerShoppingListItemsUseCase>();
builder.Services.AddTransient<RemoveCustomerShoppingListUseCase>();
// Infastructure
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/shoppingLists/{customerId}", CustomerShoppingList.GetCustomerShoppingList);
app.MapDelete("/shoppingLists/{customerId}", CustomerShoppingList.RemoveCustomerShoppingList);
app.MapPost("/shoppingLists/{customerId}/items", CustomerShoppingList.AddItemToCustomerShoppingList);
app.MapDelete("/shoppingLists/{customerId}/items", CustomerShoppingList.RemoveItemFromCustomerShoppingList);
app.MapGet("/shoppingLists/{customerId}/items", CustomerShoppingList.GetCustomerShoppingListItems);

app.UseAuthorization();

app.MapControllers();

app.Run();
