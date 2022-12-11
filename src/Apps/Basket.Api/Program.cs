using Basket.Core.Extensions;

using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints();
builder.Services.AddBasket(builder.Configuration);
builder.Services.AddAuthenticationJWTBearer(builder.Configuration["Security:Jwt:Secret"]!);
builder.Services.AddSwaggerDoc();
var app = builder.Build();

app.UseHttpLogging();

app.UseAuthentication();
app.UseAuthorization();
app.UseSwaggerGen();
app.UseFastEndpoints();
app.Run();
