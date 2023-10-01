using Catalog.Core.Model;

namespace Catalog.Core.Repository;

public sealed class Filter
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 12;
    public string? Query { get; init; }
    public decimal? PriceFrom { get; init; }
    public decimal? PriceTo { get; init; }
    public string Tag { get; init; }
    internal int Skip => (Page - 1) * PageSize;
}

public sealed class PagedResult<T>(IEnumerable<T> Data, uint Count, uint Total)
{
    public bool IsEmpty => Count == 0;

    public static PagedResult<T> Empty = new PagedResult<T>(Enumerable.Empty<T>(), 0, 0);
}

public interface IProductFilter
{
    Task<PagedResult<Product>> FilterProducts(Filter filter, CancellationToken cancellationToken = default);
}