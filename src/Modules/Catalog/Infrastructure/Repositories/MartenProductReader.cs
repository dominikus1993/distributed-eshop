using Catalog.Core.Model;
using Catalog.Core.Repository;

using Marten;

namespace Catalog.Infrastructure.Repositories;

public sealed class MartenProductReader : IProductReader
{
    private readonly IDocumentStore _store;


    public MartenProductReader(IDocumentStore store)
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