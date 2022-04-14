using OpenTelemetry.Trace;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using ShoppingList.Api.Modules;
using ShoppingList.Core.UseCases;
using ShoppingList.Infrastructure.Extensions;
using OpenTelemetry.Resources;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

AppContext.SetSwitch( "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

const string ServiceName = "shopping.list.api";
const string ServiceVersion = "1.0.0";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var otelCollector = builder.Configuration.GetConnectionString("OtelCollector");

builder.Services.AddOpenTelemetryMetrics(b =>
{
    b.AddHttpClientInstrumentation();
    b.AddAspNetCoreInstrumentation();
    b.AddMeter(nameof(CustomerShoppingList));
    b.AddOtlpExporter(options => options.Endpoint = new Uri(otelCollector));
});

builder.Services.AddOpenTelemetryTracing(b => {
    b.AddHttpClientInstrumentation();
    b.AddAspNetCoreInstrumentation();
    b.AddGrpcClientInstrumentation();
    b.SetResourceBuilder(
        ResourceBuilder.CreateDefault()
            .AddService(serviceName: ServiceName, serviceVersion: ServiceVersion));
    b.AddSource(ServiceName, nameof(GetCustomerShoppingListUseCase));
    b.AddOtlpExporter(options => options.Endpoint = new Uri(otelCollector));
});

builder.Logging.AddOpenTelemetry(b =>
{
    b.IncludeFormattedMessage = true;
    b.IncludeScopes = true;
    b.ParseStateValues = true;
    b.AddOtlpExporter(options => options.Endpoint = new Uri(otelCollector));
});


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
builder.Services.AddApiInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks().AddApiHealthCheck(builder.Configuration);
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
app.MapHealthChecks("/health", new HealthCheckOptions
{
    AllowCachingResponses = false,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapGet("/ping", () => "pong");
app.UseAuthorization();

app.MapControllers();

app.Run();
