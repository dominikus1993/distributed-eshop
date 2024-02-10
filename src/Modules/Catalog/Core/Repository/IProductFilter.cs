using Catalog.Core.Model;

namespace Catalog.Core.Repository;

public enum SortOrder
{
    Default = 0,
    PriceAsc = 1,
    PriceDesc = 2,
}

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
    public string? Tag { get; init; }
    internal int Skip => (Page - 1) * PageSize;

    public SortOrder SortOrder { get; init; } = SortOrder.Default;
}

public sealed record TagFilter(string Tag, long Count);

public sealed record TagsFilters(IReadOnlyCollection<TagFilter> Filters)
{
    internal static readonly TagsFilters Empty = new TagsFilters(Array.Empty<TagFilter>());
}

public sealed class QueryResultMetadata(TagsFilters TagFilters)
{
    internal static readonly QueryResultMetadata Empty = new(TagsFilters.Empty);
}

public sealed record PagedResult<T>(IEnumerable<T> Data, QueryResultMetadata Metadata, uint Count, uint Total)
{
    public bool IsEmpty => Count == 0;

    public static readonly PagedResult<T> Empty = new(Enumerable.Empty<T>(), QueryResultMetadata.Empty, 0, 0);
}

public interface IProductFilter
{
    Task<PagedResult<Product>> FilterProducts(Filter filter, CancellationToken cancellationToken = default);
}