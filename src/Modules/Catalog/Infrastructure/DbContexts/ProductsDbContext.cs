using Catalog.Infrastructure.Model;

using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.DbContexts;

public class ProductsDbContext : DbContext
{
    public DbSet<EfProduct> Products { get; set; }
}