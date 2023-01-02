using Catalog.Infrastructure.Model;

using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.DbContexts;

public sealed class ProductsDbContext : DbContext
{
    public DbSet<EfProduct> Products { get; set; } = null!;

    public ProductsDbContext(DbContextOptions options) : base(options)
    {
    }
    
    
}