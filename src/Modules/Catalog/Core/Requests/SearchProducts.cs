using Catalog.Core.Dto;

using Mediator;

namespace Catalog.Core.Requests;

public sealed class SearchProducts : IStreamRequest<ProductDto>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 12;
    public string? Query { get; init; }
    public decimal? PriceFrom { get; init; }
    public decimal? PriceTo { get; init; }
}