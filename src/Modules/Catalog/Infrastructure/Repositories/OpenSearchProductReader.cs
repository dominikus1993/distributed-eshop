using System.Runtime.CompilerServices;

using Catalog.Core.Model;
using Catalog.Core.Repository;
using Catalog.Infrastructure.Extensions;
using Catalog.Infrastructure.Model;

using OpenSearch.Client;
using static OpenSearch.Client.Infer;

using Id = OpenSearch.Client.Id;

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

    public async IAsyncEnumerable<Product> GetByIds(IEnumerable<ProductId> ids, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var q =
            new IdsQuery() { Values = ids.Select(x => (Id)x.Value) };

        var query = new SearchRequest(OpenSearchProductIndex.Name) { Query = q,  };
        var result = await _openSearchClient.SearchAsync<OpenSearchProduct>(query, cancellationToken);

        if (!result.IsValid || result.Total == 0)
        {
            yield break;
        }

        foreach (var product in result.Documents)
        {
            yield return product.ToProduct();
        }
    }
}