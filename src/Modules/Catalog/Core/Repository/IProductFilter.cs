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

public sealed record PricesMetaData(decimal MinPrice, decimal MaxPrice)
{
    internal static readonly PricesMetaData Empty = new PricesMetaData(0, 0);
}
public sealed record TagFilterMetaData(string Tag, long Count);

public sealed record TagsFiltersMetaData(IReadOnlyCollection<TagFilterMetaData> Filters)
{
    internal static readonly TagsFiltersMetaData Empty = new TagsFiltersMetaData(Array.Empty<TagFilterMetaData>());
}

public sealed record QueryResultMetadata(TagsFiltersMetaData TagFiltersMetaData, PricesMetaData Prices)
{
    internal static readonly QueryResultMetadata Empty = new(TagsFiltersMetaData.Empty, PricesMetaData.Empty);
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