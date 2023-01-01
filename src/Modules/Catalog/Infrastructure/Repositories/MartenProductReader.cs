using Catalog.Core.Model;
using Catalog.Core.Repository;

using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories;

public sealed class MartenProductReader : IProductReader
{
    private readonly IMongoDatabase _store;


    public MartenProductReader(IMongoDatabase store)
    {
        _store = store;
    }
    
    public Task<Product?> GetById(ProductId id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<Product> GetByIds(IEnumerable<ProductId> id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}