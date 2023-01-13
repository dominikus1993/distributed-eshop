using Carter;

namespace Catalog.Api.Modules;

public sealed class SearchProductsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/{msg}", (string msg) => TypedResults.Ok($"Hello {msg}"));
    }
}