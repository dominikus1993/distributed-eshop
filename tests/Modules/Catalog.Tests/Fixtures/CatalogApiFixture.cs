using Alba;
using Alba.Security;

using Catalog.Infrastructure.DbContexts;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.TestHost;

using Xunit;

namespace Catalog.Tests.Fixtures;

public sealed class CatalogApiFixture: IAsyncLifetime, IDisposable
{
    private readonly TestcontainerDatabaseConfiguration configuration = new PostgreSqlTestcontainerConfiguration("postgres:14-alpine") { Database = "posts", Username = "postgres", Password = "postgres" };

    public PostgreSqlTestcontainer PostgreSql { get; }
    public TestDbContextFactory DbContextFactory { get; private set; } = null!;
    public ProductsDbContext DbContext { get; private set; } = null!;
    public CatalogApiFixture()
    {
        this.PostgreSql = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(this.configuration)
            .Build();
    }
    
    public async Task<IAlbaHost> GetHost()
    {
        return await AlbaHost.For<Program>(h =>
        {
            h.UseSetting("ConnectionStrings:CatalogDb", PostgreSql.ConnectionString);
            h.ConfigureAppConfiguration((_, builder) =>
            {
                builder.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("./Api/appsettings.json", optional: false, reloadOnChange: true);
                var dict = new Dictionary<string, string>
                {
                    { "ConnectionStrings:BasketDb", PostgreSql.ConnectionString! },
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
            .UseModel(ProductsDbContextModel.Instance)
            .UseNpgsql(this.PostgreSql.ConnectionString,
                optionsBuilder =>
                {
                    optionsBuilder.EnableRetryOnFailure(5);
                    optionsBuilder.CommandTimeout(500);
                    optionsBuilder.MigrationsAssembly("Catalog");
                }).UseSnakeCaseNamingConvention();
        var context = new ProductsDbContext(builder.Options);
        DbContextFactory = new TestDbContextFactory(builder);
        DbContext = context;
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
        this.configuration.Dispose();
    }
}

[CollectionDefinition(nameof(CatalogApiFixtureCollectionTest))]
public class CatalogApiFixtureCollectionTest : ICollectionFixture<CatalogApiFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}