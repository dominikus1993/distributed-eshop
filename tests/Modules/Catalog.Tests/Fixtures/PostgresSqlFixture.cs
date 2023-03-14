using Catalog.Infrastructure.DbContexts;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

using Microsoft.EntityFrameworkCore;

using Testcontainers.PostgreSql;

using Xunit;

namespace Catalog.Tests.Fixtures;

public sealed class PostgresSqlFixture: IAsyncLifetime, IDisposable
{
    public PostgreSqlContainer PostgreSql { get; }
    public TestDbContextFactory DbContextFactory { get; private set; } = null!;
    public ProductsDbContext DbContext { get; private set; } = null!;
    public PostgresSqlFixture()
    {
        this.PostgreSql = new PostgreSqlBuilder().Build();
    }

    public async Task InitializeAsync()
    {
        await this.PostgreSql.StartAsync()
            .ConfigureAwait(false);
        var builder = new DbContextOptionsBuilder<ProductsDbContext>()
            .UseModel(ProductsDbContextModel.Instance)
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

[CollectionDefinition(nameof(PostgresSqlFixtureCollectionTests))]
public sealed class PostgresSqlFixtureCollectionTests : ICollectionFixture<PostgresSqlFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}