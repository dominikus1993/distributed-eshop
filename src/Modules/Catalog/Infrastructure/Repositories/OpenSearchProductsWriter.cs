using Catalog.Core.Model;
using Catalog.Core.Repository;
using Catalog.Infrastructure.Extensions;
using Catalog.Infrastructure.Model;

using Mediator;

using OpenSearch.Client;
using OpenSearch.Net;

namespace Catalog.Infrastructure.Repositories;

public sealed class OpenSearchProductsWriter : IProductsWriter
{
    private OpenSearchClient _openSearchClient;

    public OpenSearchProductsWriter(OpenSearchClient openSearchClient)
    {
        _openSearchClient = openSearchClient;
    }

    public async Task<AddProductResult> AddProduct(Product product, CancellationToken cancellationToken = default)
    {
        var openSearchProduct = new OpenSearchProduct(product);
        var indexReq = new IndexRequest<OpenSearchProduct>() { Document = openSearchProduct, Refresh = Refresh.True, };
        var response = await _openSearchClient.IndexAsync(indexReq, ct: cancellationToken);
        if (response.IsValid)
        {
            return Unit.Value;
        }

        return AddProductResult.Error(new InvalidOperationException("can't add product to opensearch", response.OriginalException));
    }

    public async Task<AddProductResult> AddProducts(IEnumerable<Product> products, CancellationToken cancellationToken = default)
    {
        var openSearchProduct = products.Select(product => new OpenSearchProduct(product));
        var response = await _openSearchClient.IndexManyAsync(openSearchProduct, cancellationToken: cancellationToken);
        if (response.IsValid)
        {
            return Unit.Value;
        }

        return AddProductResult.Error(new InvalidOperationException("can't add products to opensearch", response.OriginalException));
    }

    public async Task RemoveAllProducts(CancellationToken cancellationToken = default)
    {
        var result = 
            await _openSearchClient.DeleteByQueryAsync<OpenSearchProduct>(q => q.Index(OpenSearchProductIndex.SearchIndex).Query(d => d.MatchAll()), cancellationToken);
        if (!result.IsValid)
        {
            throw new InvalidOperationException("remove documents error");
        }
    }
}