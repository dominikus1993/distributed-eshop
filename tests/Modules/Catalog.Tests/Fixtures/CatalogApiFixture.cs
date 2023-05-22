using Alba;
using Alba.Security;

using Catalog.Core.Repository;
using Catalog.Infrastructure.DbContexts;
using Catalog.Infrastructure.Repositories;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.TestHost;

using Testcontainers.PostgreSql;

using Xunit;

namespace Catalog.Tests.Fixtures;

public sealed class CatalogApiFixture: IAsyncLifetime, IDisposable
{
    public PostgreSqlContainer PostgreSql { get; }=  new PostgreSqlBuilder().Build();
    public TestDbContextFactory DbContextFactory { get; private set; } = null!;
    public ProductsDbContext DbContext { get; private set; } = null!;
    
    public IProductsWriter ProductsWriter { get; private set; }
    public CatalogApiFixture()
    {
    }
    
    public async Task<IAlbaHost> GetHost()
    {
        return await AlbaHost.For<Program>(h =>
        {
            h.UseSetting("ConnectionStrings:CatalogDb", PostgreSql.GetConnectionString());
            h.ConfigureAppConfiguration((_, builder) =>
            {
                builder.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("./Api/appsettings.json", optional: false, reloadOnChange: true);
                var dict = new Dictionary<string, string>
                {
                    { "ConnectionStrings:BasketDb", PostgreSql.GetConnectionString()! },
                };
                builder.AddInMemoryCollection(dict!);
            });
        });
    }

    public async Task InitializeAsync()
    {
        await this.PostgreSql.StartAsync()
            .ConfigureAwait(false);
        var builder = new DbContextOptionsBuilder<ProductsDbContext>()
            // .UseModel(ProductsDbContextModel.Instance)
            .UseNpgsql(this.PostgreSql.GetConnectionString(),
                optionsBuilder =>
                {
                    optionsBuilder.EnableRetryOnFailure(5);
                    optionsBuilder.CommandTimeout(500);
                    optionsBuilder.MigrationsAssembly("Catalog");
                }).UseSnakeCaseNamingConvention();
        var context = new ProductsDbContext(builder.Options);
        DbContextFactory = new TestDbContextFactory(builder);
        DbContext = context;
        ProductsWriter = new EfCoreProductsWriter(DbContextFactory);
        await context.Database.MigrateAsync();

    }

    public async Task DisposeAsync()
    {
        await this.PostgreSql.DisposeAsync()
            .ConfigureAwait(false);
        await DbContext.DisposeAsync();
    }

    public void Dispose()
    {
    }
}

[CollectionDefinition(nameof(CatalogApiFixtureCollectionTest))]
public class CatalogApiFixtureCollectionTest : ICollectionFixture<CatalogApiFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}