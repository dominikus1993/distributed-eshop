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

// Infastructure
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
