using Catalog.Infrastructure.DbContexts;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace Catalog.Tests.Fixtures;

public sealed class PostgresSqlFixture: IAsyncLifetime, IDisposable
{
    private readonly TestcontainerDatabaseConfiguration configuration = new PostgreSqlTestcontainerConfiguration("postgres:14-alpine") { Database = "posts", Username = "postgres", Password = "postgres" };

    public PostgreSqlTestcontainer PostgreSql { get; }
    public ProductsDbContext DbContext { get; private set; } = null!;
    public PostgresSqlFixture()
    {
        this.PostgreSql = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(this.configuration)
            .Build();
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
        this.DbContext = new ProductsDbContext(builder.Options);
        await this.DbContext.Database.MigrateAsync();

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

public sealed class TestDbContextFactory : IDbContextFactory<ProductsDbContext>
{
    private readonly ProductsDbContext _context;

    public TestDbContextFactory(ProductsDbContext context)
    {
        _context = context;
    }

    public ProductsDbContext CreateDbContext()
    {
        return _context;
    }
}

[CollectionDefinition(nameof(ProductContextContextCollection))]
public class ProductContextContextCollection : ICollectionFixture<PostgresSqlFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}