using System.Runtime.CompilerServices;

using Catalog.Core.Model;
using Catalog.Core.Repository;
using Catalog.Infrastructure.DbContexts;

using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public sealed class EfCoreProductReader : IProductReader
{
    private readonly IDbContextFactory<ProductsDbContext> _storeFactory;


    public EfCoreProductReader(IDbContextFactory<ProductsDbContext> store)
    {
        _storeFactory = store;
    }
    
    public async Task<Product?> GetById(ProductId id, CancellationToken cancellationToken = default)
    {
        await using var context = await _storeFactory.CreateDbContextAsync(cancellationToken);
        var result = await context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == id, cancellationToken: cancellationToken);
        return result?.ToProduct();
    }

    public async IAsyncEnumerable<Product> GetByIds(IEnumerable<ProductId> ids, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using var context = await _storeFactory.CreateDbContextAsync(cancellationToken);
        var result = context.Products.AsNoTracking().Where(product => ids.Contains(product.ProductId)).AsAsyncEnumerable();
        await foreach (var product in result.WithCancellation(cancellationToken))
        {
            yield return product.ToProduct();
        }
    }
}