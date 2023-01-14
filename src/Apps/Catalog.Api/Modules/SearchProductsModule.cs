using Carter;

using Mediator;

namespace Catalog.Api.Modules;

public struct SearchProductsRequest
{
    public SearchProductsRequest(ISender sender)
    {
        Sender = sender;
        Query = null;
        PriceFrom = null;
        PriceTo = null;
    }

    public int? Page { get; init; } = 1;
    public int? PageSize { get; init; } = 12;
    public string? Query { get; init; }
    public decimal? PriceFrom { get; init; }
    public decimal? PriceTo { get; init; }
    
    public ISender Sender { get; init; }

    public override string ToString()
    {
        return $"{nameof(Page)}: {Page}, {nameof(PageSize)}: {PageSize}, {nameof(Query)}: {Query}, {nameof(PriceFrom)}: {PriceFrom}, {nameof(PriceTo)}: {PriceTo}";
    }
}

public sealed class SearchProductsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", ([AsParameters]SearchProductsRequest msg) => TypedResults.Ok($"Hello {msg}"));
    }
}