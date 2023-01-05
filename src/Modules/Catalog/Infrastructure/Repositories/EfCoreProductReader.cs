using Catalog.Core.Model;
using Catalog.Core.Repository;
using Catalog.Infrastructure.DbContexts;

using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public sealed class EfCoreProductReader : IProductReader
{
    private readonly ProductsDbContext _store;


    public EfCoreProductReader(ProductsDbContext store)
    {
        _store = store;
    }
    
    public async Task<Product?> GetById(ProductId id, CancellationToken cancellationToken = default)
    {
        EqualityComparer<ProductId>.Default;
        var result = await _store.Products.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == id.Value, cancellationToken: cancellationToken);
        return result?.ToProduct();
    }

    public IAsyncEnumerable<Product> GetByIds(IEnumerable<ProductId> id, CancellationToken cancellationToken = default)
    {
        var result = await _store.Products.AsNoTracking().Where(product => id.Contains(product.ProductId));
    }
}