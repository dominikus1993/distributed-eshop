using Carter;

using Catalog.Core.Dto;
using Catalog.Core.Model;
using Catalog.Core.Requests;

using FluentValidation;

using Mediator;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Modules;

public sealed class SearchProductsRequestValidator : AbstractValidator<SearchProductsRequest>
{
    public SearchProductsRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0).WithMessage("Invalid page number");
        RuleFor(x => x.PageSize).GreaterThan(0).WithMessage("Invalid page size");
        RuleFor(x => x.PriceFrom).GreaterThanOrEqualTo(0).WithMessage("Invalid price from");
        RuleFor(x => x.PriceTo).GreaterThanOrEqualTo(0).WithMessage("Invalid price to");
        RuleFor(x => x.PriceTo).GreaterThanOrEqualTo(x => x.PriceFrom).When(x => x.PriceTo.HasValue).WithMessage("Invalid price range");
        RuleFor(x => x.Query).Length(1, 100).When(x => !string.IsNullOrEmpty(x.Query)).WithMessage("Query is too long");
    }
}

public sealed class SearchProductsRequest
{

    public int? Page { get; init; } = 1;
    public int? PageSize { get; init; } = 12;
    public string? Query { get; init; }
    public decimal? PriceFrom { get; init; }
    public decimal? PriceTo { get; init; }
    public ISender Sender { get; init; } = null!;

    public override string ToString()
    {
        return $"{nameof(Page)}: {Page}, {nameof(PageSize)}: {PageSize}, {nameof(Query)}: {Query}, {nameof(PriceFrom)}: {PriceFrom}, {nameof(PriceTo)}: {PriceTo}";
    }
}

public sealed record PagedProductsResult(IEnumerable<ProductDto> Products, uint Count, uint Total);

public sealed class SearchProductsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products", SearchProducts);
        app.MapGet("/api/products/{id:guid}", GetProductById);
    }

    
    private static async Task<Results<Ok<ProductDto>, NotFound>> GetProductById(Guid id, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProductById(ProductId.From(id)), cancellationToken);
        if (result is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result);
    }
    
    private static async Task<Results<Ok<PagedProductsResult>, NoContent, BadRequest<ProblemDetails>>> SearchProducts([AsParameters]SearchProductsRequest request, [FromServices]IValidator<SearchProductsRequest> validator, CancellationToken cancellationToken)
    {

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.BadRequest(new ProblemDetails()
            {
                Detail = validationResult.ToString(),
                Title = "Validation error",
                Status = StatusCodes.Status400BadRequest
            });
        }
        
        var query = new SearchProducts()
        {
            PriceFrom = request.PriceFrom,
            PriceTo = request.PriceTo,
            Query = request.Query,
            Page = request.Page ?? 1,
            PageSize = request.PageSize ?? 12
        };

        var result = await request.Sender.Send(query, cancellationToken);

        if (result is { Count: 0 })
        {
            return TypedResults.NoContent();
        }

        return TypedResults.Ok(new PagedProductsResult(result.Data, result.Count, result.Total));
    }
}