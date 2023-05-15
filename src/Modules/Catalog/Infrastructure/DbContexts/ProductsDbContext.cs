using Catalog.Core.Model;
using Catalog.Infrastructure.DbContexts.Configuration;
using Catalog.Infrastructure.Model;

using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.DbContexts;

public sealed class ProductsDbContext : DbContext
{
    public DbSet<EfProduct> Products { get; set; } = null!;

    public ProductsDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    private static Func<ProductsDbContext, ProductId, CancellationToken, Task<EfProduct?>> GetById =
        EF.CompileAsyncQuery(
            (ProductsDbContext context, ProductId id, CancellationToken cancellationToken) =>
                context.Products.AsNoTracking().FirstOrDefault(c => c.ProductId == id));
    
    public async Task<EfProduct?> GetProductById(ProductId id, CancellationToken cancellationToken = default)
    {
        return await GetById(this, id, cancellationToken);
    }
}