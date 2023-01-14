using Catalog.Core.Repository;
using Catalog.Infrastructure.DbContexts;
using Catalog.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddTransient<IProductFilter, EfCoreProductFilter>();
        services.AddTransient<IProductReader, EfCoreProductReader>();
        services.AddPooledDbContextFactory<ProductsDbContext>(builder =>
        {
            builder.UseModel(ProductsDbContextModel.Instance);
            builder.UseNpgsql(configuration.GetConnectionString("CatalogDb"), optionsBuilder =>
            {
                optionsBuilder.EnableRetryOnFailure(5);
                optionsBuilder.CommandTimeout(720);
                optionsBuilder.MigrationsAssembly(typeof(ProductsDbContext).Assembly.FullName);
            }).UseSnakeCaseNamingConvention();
        });
        return services;
    }
}