using Catalog.Core.Repository;
using Catalog.Tests.Api.WebApp;
using Catalog.Tests.Testcontainers;

using OpenSearch.Client;

using Xunit.Abstractions;

namespace Catalog.Tests.Fixtures;

public sealed class CatalogApiFixture: IAsyncLifetime, IDisposable
{
    public OpenSearchContainer OpenSearch { get; } = new OpenSearchBuilder().Build();
    private IOpenSearchClient OpenSearchClient { get; set; }
    public IProductsWriter ProductsWriter { get; private set; }
    public CatalogApiFixture()
    {
    }
    
    public async Task InitializeAsync()
    {
        await this.OpenSearch.StartAsync()
            .ConfigureAwait(false);
    }

    public async Task DisposeAsync()
    {
        await this.OpenSearch.DisposeAsync();
    }

    public CatalogApiWebApplicationFactory CatalogApi(ITestOutputHelper helper) =>
        new CatalogApiWebApplicationFactory(OpenSearch.GetConfiguration(), helper);

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