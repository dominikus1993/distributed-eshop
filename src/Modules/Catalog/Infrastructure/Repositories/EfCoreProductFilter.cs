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
    
    public async Task<PagedResult<Product>> FilterProducts(Filter filter, CancellationToken cancellationToken = default)
    {
        await using var context = await _storeFactory.CreateDbContextAsync(cancellationToken);
        var query = context.Products.AsNoTracking().AsQueryable();
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

        var count = await query.CountAsync(cancellationToken: cancellationToken);
        var result = await query.OrderBy(x => x.DateCreated).Skip(filter.Skip).Take(filter.PageSize).ToListAsync(cancellationToken: cancellationToken);
        if (result.Count == 0)
        {
            return PagedResult<Product>.Empty;
        }
        return new PagedResult<Product>(result.Select(x => x.ToProduct()), (uint)result.Count, (uint)count);
    }
}