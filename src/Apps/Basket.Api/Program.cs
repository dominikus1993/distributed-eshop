using Basket.Core.Extensions;
using Basket.Infrastructure.Extensions;

using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints();
builder.AddBasket();
builder.AddBasketInfrastructure();
builder.Services.AddAuthenticationJWTBearer(builder.Configuration["Security:Jwt:Secret"]!);
builder.Services.AddSwaggerDoc();

var app = builder.Build();

app.UseHttpLogging();

app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();
app.UseSwaggerGen();
app.Run();

public partial class Program {}