using System.Runtime.CompilerServices;

using Catalog.Core.Model;
using Catalog.Core.Repository;
using Catalog.Infrastructure.Extensions;
using Catalog.Infrastructure.Model;

using OpenSearch.Client;
using static OpenSearch.Client.Infer;

namespace Catalog.Infrastructure.Repositories;

public sealed class OpenSearchProductFilter : IProductFilter
{
    private readonly IOpenSearchClient _openSearchClient;

    public OpenSearchProductFilter(IOpenSearchClient openSearchClient)
    {
        _openSearchClient = openSearchClient;
    }
    
    public async IAsyncEnumerable<Product> FilterProducts(Filter filter, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var searchRequest = new SearchRequest<OpenSearchProduct>(OpenSearchProductIndex.Name)
        {
            Size = filter.PageSize,
            From = filter.Skip,
        };
        if (!string.IsNullOrEmpty(filter.Query))
        {
            searchRequest.Query &= new MatchQuery()
            {
                Field = Field(OpenSearchProductIndex.SearchIndex),
                Query = filter.Query,
                Operator = Operator.And,
                Fuzziness = Fuzziness.Auto,
            };
        }
        
        if (!string.IsNullOrEmpty(filter.Tag))
        {
            searchRequest.Query &= new MatchQuery()
            {
                Field = Field(OpenSearchProductIndex.TagsKeyword),
                Query = filter.Tag,
                Operator = Operator.And,
                Fuzziness = Fuzziness.Auto,
            };
        }

        if (filter.PriceFrom.HasValue)
        {
            var priceFrom = decimal.ToDouble(filter.PriceFrom.Value);
            var promotionalPriceQ = new NumericRangeQuery()
            {
                Field = Field<OpenSearchProduct>(static p => p.PromotionalPrice),
                GreaterThanOrEqualTo = priceFrom,
                Name = "promotional priceFrom query"
            };
            var priceQ = new NumericRangeQuery()
            {
                Field = Field<OpenSearchProduct>(static p => p.Price),
                GreaterThanOrEqualTo = priceFrom,
                Name = "priceFrom query"
            };

            searchRequest.Query &= (promotionalPriceQ || priceQ);
        }
        
        if (filter.PriceTo.HasValue)
        {
            var priceTo = decimal.ToDouble(filter.PriceTo.Value);
            var promotionalPriceQ = new NumericRangeQuery()
            {
                Field = Field<OpenSearchProduct>(static p => p.PromotionalPrice),
                LessThanOrEqualTo = priceTo,
                Name = "promotional priceTo query"
            };
            var priceQ = new NumericRangeQuery()
            {
                Field = Field<OpenSearchProduct>(static p => p.Price),
                LessThanOrEqualTo = priceTo,
                Name = "priceTo query"
            };

            searchRequest.Query &= (promotionalPriceQ || priceQ);
        }

        var result = await _openSearchClient.SearchAsync<OpenSearchProduct>(searchRequest, cancellationToken);

        if (!result.IsValid || result.Total == 0)
        {
            yield break;
        }

        foreach (var openSearchProduct in result.Documents)
        {
            yield return openSearchProduct.ToProduct();
        }
    }
}