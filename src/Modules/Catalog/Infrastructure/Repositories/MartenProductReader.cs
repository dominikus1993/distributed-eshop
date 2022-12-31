using Catalog.Core.Model;
using Catalog.Core.Repository;

namespace Catalog.Infrastructure.Repositories;

public class MartenProductReader : IProductReader
{
    public Task<Product?> GetById(ProductId id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Product?> GetByIds(IEnumerable<ProductId> id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}