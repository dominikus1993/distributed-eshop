
using Carter;

using Catalog.Api.Modules;
using Catalog.Core.Extensions;
using Catalog.Infrastructure.Extensions;

using FluentValidation;

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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog.Api", Version = "v1" });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpLogging();

app.UseAuthentication();
app.UseAuthorization();
app.MapCarter();
app.MapSwagger();
app.MapHealthCheckEndpoints();

app.Run();

public partial class Program {}