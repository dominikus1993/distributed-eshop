using System.Linq.Expressions;

using Catalog.Core.Model;
using Catalog.Infrastructure.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Catalog.Infrastructure.DbContexts.Configuration;

internal sealed class ProductIdConverter : ValueConverter<ProductId, Guid>
{
    public ProductIdConverter()
        : base(
            v => v.Value,
            v => new ProductId(v))
    {
    }
}

internal class UtcTimeConverter : ValueConverter<DateTime, DateTime>
{
    public UtcTimeConverter()
        : base(
            v => v,
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
    {
    }
}

public class ProductConfiguration : IEntityTypeConfiguration<EfProduct>
{
    public void Configure(EntityTypeBuilder<EfProduct> builder)
    {
        builder.HasKey(product => product.ProductId);
        builder.HasIndex(product => product.DateCreated);
        builder.HasIndex(product => new { product.Price, product.PromotionalPrice });
        builder.Property(product => product.ProductId).HasConversion<ProductIdConverter>();
        builder.Property(product => product.Name).IsRequired();
        builder.Property(product => product.Description).IsRequired();
        builder.Property(product => product.DateCreated)
            .HasDefaultValueSql("(now() at time zone 'utc')")
            .HasConversion<UtcTimeConverter>();
        builder
            .Property(p => p.Version)
            .IsRowVersion();
        builder
            .HasGeneratedTsVectorColumn(
                p => p.SearchVector,
                "english",  
                p => new { p.Name, p.Description })  
            .HasIndex(p => p.SearchVector)
            .HasMethod("GIN");
    }
}