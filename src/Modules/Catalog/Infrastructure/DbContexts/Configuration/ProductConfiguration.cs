using Catalog.Infrastructure.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.DbContexts.Configuration;

public class ProductConfiguration : IEntityTypeConfiguration<EfProduct>
{
    public void Configure(EntityTypeBuilder<EfProduct> builder)
    {
        builder.HasKey(product => product.ProductId);
    }
}