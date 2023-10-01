using Catalog.Core.Repository;
using Catalog.Infrastructure.Extensions;
using Catalog.Infrastructure.Repositories;
using Catalog.Tests.Testcontainers;

using OpenSearch.Client;

namespace Catalog.Tests.Fixtures;

public sealed class OpenSearchFixture : IAsyncLifetime
{
    public OpenSearchContainer Container { get; } = new OpenSearchBuilder().Build();
    
    private IOpenSearchClient OpenSearchClient { get; set; }
    public IProductsWriter ProductsWriter { get; private set; }
    public IProductReader ProductReader { get; private set; }
    public IProductFilter ProductFilter { get; private set; }
    
    public async Task InitializeAsync()
    {
        await Container.StartAsync();
        OpenSearchClient = OpenSearchInstaller.Setup(Container.GetConfiguration());
        await OpenSearchInstaller.CreateIndexIfNotExists(OpenSearchClient);
        ProductsWriter = new OpenSearchProductsWriter(OpenSearchClient);
        ProductReader = new OpenSearchProductReader(OpenSearchClient);
        ProductFilter = new OpenSearchProductFilter(OpenSearchClient);
    }

    public async Task Clean()
    {
        await OpenSearchClient.DeleteByQueryAsync(new DeleteByQueryRequest(OpenSearchProductIndex.Name) { Query = new MatchAllQuery() });
        await OpenSearchClient.Indices.RefreshAsync();
    } 

    public Task DisposeAsync()
    {
        return Container.StopAsync();
    }
}