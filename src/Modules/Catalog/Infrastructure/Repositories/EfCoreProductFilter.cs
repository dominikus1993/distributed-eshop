using System.Runtime.CompilerServices;

using Catalog.Core.Model;
using Catalog.Core.Repository;
using Catalog.Infrastructure.DbContexts;

using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public sealed class EfCoreProductFilter : IProductFilter
{
    private readonly IDbContextFactory<ProductsDbContext> _storeFactory;


    public EfCoreProductFilter(IDbContextFactory<ProductsDbContext> storeFactory)
    {
        _storeFactory = storeFactory;
    }
    
    public async IAsyncEnumerable<Product> FilterProducts(Filter filter, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using var context = await _storeFactory.CreateDbContextAsync(cancellationToken);
        var query = context.Products.AsNoTracking();
        if (!string.IsNullOrEmpty(filter.Query))
        {
            query = query.Where(x => x.SearchVector.Matches(filter.Query));
        }
        
        if (!string.IsNullOrEmpty(filter.Tag))
        {
            query = query.Where(x => x.SearchVector.Matches(filter.Tag));
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

        var result = await query.OrderBy(x => x.DateCreated).Skip(filter.Skip).Take(filter.PageSize).ToListAsync();

        foreach (var product in result)
        {
            yield return product.ToProduct();
        }
    }
}