using Carter;

using Catalog.Core.Dto;
using Catalog.Core.Model;
using Catalog.Core.Requests;

using Mediator;

using Microsoft.AspNetCore.Http.HttpResults;

namespace Catalog.Api.Modules;

public sealed class SearchProductsRequest
{

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
        app.MapGet("/products", ([AsParameters]SearchProductsRequest msg) => TypedResults.Ok($"Hello {msg}"));
        app.MapGet("/products/{id:guid}", GetProductById);
    }

    
    public static async Task<Results<Ok<ProductDto>, NotFound>> GetProductById(Guid id, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProductById(ProductId.From(id)), cancellationToken);
        if (result is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result);
    }
    
    public static async Task<Results<Ok<IReadOnlyCollection<ProductDto>>, NoContent>> SearchProducts([AsParameters]SearchProductsRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        return TypedResults.NoContent();
    }
}