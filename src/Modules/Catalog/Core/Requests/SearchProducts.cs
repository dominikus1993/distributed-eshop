using Catalog.Core.Dto;
using Catalog.Core.Repository;

using Mediator;

namespace Catalog.Core.Requests;

public sealed class SearchProducts : IRequest<PagedResult<ProductDto>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 12;
    public string? Query { get; init; }
    public decimal? PriceFrom { get; init; }
    public decimal? PriceTo { get; init; }
}