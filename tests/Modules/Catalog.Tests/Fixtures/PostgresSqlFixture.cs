using Catalog.Infrastructure.Configuration;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

using Marten;

namespace Catalog.Tests.Fixtures;

public sealed class PostgresSqlFixture
{
    private readonly TestcontainerDatabaseConfiguration configuration = new PostgreSqlTestcontainerConfiguration("postgres:14-alpine") { Database = "posts", Username = "postgres", Password = "postgres" };

    public PostgreSqlTestcontainer PostgreSql { get; }
    internal DocumentStore Store { get; private set; } = null!;

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
        Store = DocumentStore.For(MartenDocumentStoreConfig.Configure(PostgreSql.ConnectionString, true));
    }

    public async Task DisposeAsync()
    {
        await this.PostgreSql.DisposeAsync()
            .ConfigureAwait(false);
    }

    public void Dispose()
    {
        Store.Dispose();
        this.configuration.Dispose();
    }
}