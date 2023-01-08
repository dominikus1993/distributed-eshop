using System.Runtime.CompilerServices;

using Catalog.Core.Model;
using Catalog.Core.Repository;
using Catalog.Infrastructure.DbContexts;

using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public sealed class EfCoreProductFilter : IProductFilter
{
    private readonly ProductsDbContext _store;


    public EfCoreProductFilter(ProductsDbContext store)
    {
        _store = store;
    }
    
    public async IAsyncEnumerable<Product> FilterProducts(Filter filter, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var query = _store.Products.AsNoTracking();
        if (!string.IsNullOrEmpty(filter.Query))
        {
            query = query.Where(x => x.SearchVector.Matches(filter.Query));
        }

        if (filter.PriceFrom.HasValue)
        {
            query = query.Where(product => product.PromotionalPrice.HasValue ? product.PromotionalPrice >= filter.PriceFrom.Value : product.Price >= filter.PriceFrom.Value);
        }
        
        if (filter.PriceTo.HasValue)
        {
            query = query.Where(product =>
                product.PromotionalPrice.HasValue
                    ? product.PromotionalPrice <= filter.PriceTo.Value
                    : product.Price <= filter.PriceTo.Value);
        }

        var result = query.Skip(filter.Skip).Take(filter.PageSize);

        await foreach (var product in result.AsAsyncEnumerable().WithCancellation(cancellationToken))
        {
            yield return product.ToProduct();
        }
    }
}