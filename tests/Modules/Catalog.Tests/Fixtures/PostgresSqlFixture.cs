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
    public TestDbContextFactory DbContextFactory { get; private set; } = null!;
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

public sealed class TestDbContextFactory : IDbContextFactory<ProductsDbContext>
{
    private readonly  DbContextOptionsBuilder<ProductsDbContext> _builder;

    public TestDbContextFactory(DbContextOptionsBuilder<ProductsDbContext> builder)
    {
        _builder = builder;
    }

    public ProductsDbContext CreateDbContext()
    {
        return new ProductsDbContext(_builder.Options);
    }
}

[CollectionDefinition(nameof(ProductContextContextCollection))]
public class ProductContextContextCollection : ICollectionFixture<PostgresSqlFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}