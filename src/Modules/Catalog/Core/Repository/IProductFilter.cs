using Catalog.Core.Model;

namespace Catalog.Core.Repository;

public readonly record struct Filter(int Page = 1, int PageSize = 12);

public interface IProductFilter
{
    IAsyncEnumerable<Product> FilterProducts(Filter filter, CancellationToken cancellationToken = default);
}