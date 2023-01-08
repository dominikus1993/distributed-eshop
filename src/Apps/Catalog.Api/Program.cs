
using Catalog.Infrastructure.Extensions;

using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;

using Hosting.HealthChecks;

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
builder.Services.AddFastEndpoints();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJWTBearerAuth(builder.Configuration["Security:Jwt:Secret"]!);
builder.Services.AddSwaggerDoc();
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