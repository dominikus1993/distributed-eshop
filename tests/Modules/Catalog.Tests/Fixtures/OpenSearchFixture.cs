using Catalog.Core.Repository;
using Catalog.Infrastructure.Extensions;
using Catalog.Infrastructure.Repositories;
using Catalog.Tests.Testcontainers;

namespace Catalog.Tests.Fixtures;

public sealed class OpenSearchFixture : IAsyncLifetime
{
    public OpenSearchContainer Container { get; } = new OpenSearchBuilder().Build();
    public IProductsWriter ProductsWriter { get; private set; }
    
    public async Task InitializeAsync()
    {
        await Container.StartAsync();
        var client = OpenSearchInstaller.Setup(Container.GetConfiguration());
        await OpenSearchInstaller.CreateIndexIfNotExists(client);
        ProductsWriter = new OpenSearchProductsWriter(client);
    }

    public async Task DisposeAsync()
    {
        await Container.StopAsync();
    }
}