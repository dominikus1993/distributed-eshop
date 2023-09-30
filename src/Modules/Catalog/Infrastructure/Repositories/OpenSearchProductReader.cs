using Catalog.Core.Model;
using Catalog.Core.Repository;
using Catalog.Infrastructure.Extensions;
using Catalog.Infrastructure.Model;

using OpenSearch.Client;

namespace Catalog.Infrastructure.Repositories;

public sealed class OpenSearchProductReader : IProductReader
{
    private readonly IOpenSearchClient _openSearchClient;

    public OpenSearchProductReader(IOpenSearchClient openSearchClient)
    {
        _openSearchClient = openSearchClient;
    }
    
    public async Task<Product?> GetById(ProductId id, CancellationToken cancellationToken = default)
    {
        Id searchId = id.Value;
        var productQueryResult =
            await _openSearchClient.GetAsync<OpenSearchProduct>(new GetRequest(OpenSearchProductIndex.Name, searchId),
                cancellationToken);

        if (productQueryResult.Found)
        {
            return productQueryResult.Source.ToProduct();
        }

        return null;
    }

    public IAsyncEnumerable<Product> GetByIds(IEnumerable<ProductId> ids, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}