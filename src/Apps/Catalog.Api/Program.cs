
using Carter;

using Catalog.Api.Modules;
using Catalog.Core.Extensions;
using Catalog.Infrastructure.Extensions;

using FluentValidation;

using Hosting.Extensions;
using Hosting.HealthChecks;

using Microsoft.OpenApi.Models;

using Npgsql;

using Telemetry.OpenTelemetry;

var service = new Service() { Name = "Catalog.Api", Version = "1.0.0" };
var builder = WebApplication.CreateBuilder(args);
builder.AddOpenTelemetry(service)
    .AddOpenTelemetryTracing(b =>
    {
        b.AddNpgsql();
    })
    .AddOpenTelemetryMetrics();
builder.Services.AddTransient<IValidator<SearchProductsRequest>, SearchProductsRequestValidator>();
builder.Services.AddCatalog();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication();
builder.Services.AddCarter();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocument();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseHttpLogging();

app.UseAuthentication();
app.UseAuthorization();
app.MapCarter();
app.UseSwaggerUi();
app.MapHealthCheckEndpoints();

app.Run();

public partial class Program {}