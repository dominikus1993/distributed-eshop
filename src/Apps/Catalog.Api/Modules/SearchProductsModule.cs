using Carter;

namespace Catalog.Api.Modules;

public sealed class SearchProductsRequest
{
    public int? Page { get; init; } = 1;
    public int? PageSize { get; init; } = 12;
    public string? Query { get; init; }
    public decimal? PriceFrom { get; init; }
    public decimal? PriceTo { get; init; }
}

public sealed class SearchProductsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", ([AsParameters]SearchProductsRequest msg) => TypedResults.Ok($"Hello {msg}"));
    }
}