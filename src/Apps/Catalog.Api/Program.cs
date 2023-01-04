
using Catalog.Infrastructure.Extensions;

using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJWTBearerAuth(builder.Configuration["Security:Jwt:Secret"]!);
builder.Services.AddSwaggerDoc();

var app = builder.Build();

app.UseHttpLogging();

app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();
app.UseSwaggerGen();
app.Run();

public partial class Program {}