using Catalog.Core.Model;

namespace Catalog.Core.Repository;

public sealed class Filter
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 12;
    public string? Query { get; init; }
    public decimal? PriceFrom { get; init; }
    public decimal? PriceTo { get; init; }
    internal int Skip => (Page - 1) * PageSize;
}

public interface IProductFilter
{
    IAsyncEnumerable<Product> FilterProducts(Filter filter, CancellationToken cancellationToken = default);
}