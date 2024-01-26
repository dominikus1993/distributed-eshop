using Basket.Core.Extensions;
using Basket.Infrastructure.Extensions;

using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;

using Hosting.HealthChecks;

using OpenTelemetry.Trace;

using Telemetry.OpenTelemetry;

var service = new Service() { Name = "Basket.Api", Version = "1.0.0" };
var builder = WebApplication.CreateBuilder(args);
builder.AddOpenTelemetry(service)
    .AddOpenTelemetryTracing(b =>
    {
        b.AddRedisInstrumentation();
    })
    .AddOpenTelemetryMetrics();

builder.Services.AddFastEndpoints();
builder.AddBasket();
builder.AddBasketInfrastructure();
builder.Services.AddJWTBearerAuth(builder.Configuration["Security:Jwt:Secret"]!);
builder.Services.AddSwaggerDocument();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseHttpLogging();

app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();
app.UseSwaggerGen();
app.MapHealthCheckEndpoints();
app.Run();

public partial class Program {}