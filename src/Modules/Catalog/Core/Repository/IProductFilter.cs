using Catalog.Core.Model;

namespace Catalog.Core.Repository;

public sealed class Filter
{
    private readonly int _page = 1;
    public int Page
    {
        get => _page;
        init
        {
            _page = value < 1 ? 1 : value;
        }
    }

    public int PageSize { get; init; } = 12;
    public string? Query { get; init; }
    public decimal? PriceFrom { get; init; }
    public decimal? PriceTo { get; init; }
    public string Tag { get; init; }
    internal int Skip => (Page - 1) * PageSize;
}

public sealed record PagedResult<T>(IEnumerable<T> Data, uint Count, uint Total)
{
    public bool IsEmpty => Count == 0;

    public static PagedResult<T> Empty = new(Enumerable.Empty<T>(), 0, 0);
}

public interface IProductFilter
{
    Task<PagedResult<Product>> FilterProducts(Filter filter, CancellationToken cancellationToken = default);
}