using System.Runtime.CompilerServices;

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
        var result = await _store.Products.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == id, cancellationToken: cancellationToken);
        return result?.ToProduct();
    }

    public async IAsyncEnumerable<Product> GetByIds(IEnumerable<ProductId> id, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var result = _store.Products.AsNoTracking().Where(product => id.Contains(product.ProductId)).AsAsyncEnumerable();
        await foreach (var product in result.WithCancellation(cancellationToken))
        {
            yield return product.ToProduct();
        }
    }
}