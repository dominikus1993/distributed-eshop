using System.Runtime.CompilerServices;

using Catalog.Core.Model;
using Catalog.Core.Repository;
using Catalog.Infrastructure.Extensions;
using Catalog.Infrastructure.Model;

using OpenSearch.Client;
using static OpenSearch.Client.Infer;

using SortOrder = Catalog.Core.Repository.SortOrder;

namespace Catalog.Infrastructure.Repositories;

public sealed class OpenSearchProductFilter : IProductFilter
{
    private readonly IOpenSearchClient _openSearchClient;

    public OpenSearchProductFilter(IOpenSearchClient openSearchClient)
    {
        _openSearchClient = openSearchClient;
    }

    public async Task<PagedResult<Product>> FilterProducts(Filter filter, CancellationToken cancellationToken = default)
    {
        var searchRequest = new SearchRequest<OpenSearchProduct>(OpenSearchProductIndex.Name)
        {
            Size = filter.PageSize, From = filter.Skip,
            Sort = GetSortOrder(filter.SortOrder),
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
                Fuzziness = Fuzziness.EditDistance(0),
            };
        }

        if (filter.PriceFrom.HasValue)
        {
            var priceFrom = decimal.ToDouble(filter.PriceFrom.Value);
            var priceQ = new NumericRangeQuery()
            {
                Field = Field<OpenSearchProduct>(static p => p.SalePrice),
                GreaterThanOrEqualTo = priceFrom,
                Name = "priceFrom query"
            };

            searchRequest.Query &= priceQ;
        }

        if (filter.PriceTo.HasValue)
        {
            var priceTo = decimal.ToDouble(filter.PriceTo.Value);
            var priceQ = new NumericRangeQuery()
            {
                Field = Field<OpenSearchProduct>(static p => p.SalePrice),
                LessThanOrEqualTo = priceTo,
                Name = "priceTo query"
            };

            searchRequest.Query &= priceQ;
        }

        var result = await _openSearchClient.SearchAsync<OpenSearchProduct>(searchRequest, cancellationToken);

        if (!result.IsValid || result.Total == 0)
        {
            return PagedResult<Product>.Empty;
        }

        var res = result.Documents.Select(x => x.ToProduct()).ToArray();

        return new PagedResult<Product>(res, QueryResultMetadata.Empty, (uint)res.Length, (uint)result.Total);
    }

    private static ISort[] GetSortOrder(SortOrder sortOrder)
    {
        return sortOrder switch
        {
            SortOrder.Default => Array.Empty<ISort>(),
            SortOrder.PriceAsc => new ISort[]
            {
                new FieldSort()
                {
                    Field = Field<OpenSearchProduct>(static p => p.SalePrice),
                    Order = OpenSearch.Client.SortOrder.Ascending
                }
            },
            SortOrder.PriceDesc => new ISort[]
            {
                new FieldSort()
                {
                    Field = Field<OpenSearchProduct>(static p => p.SalePrice),
                    Order = OpenSearch.Client.SortOrder.Descending
                }
            },
            _ => throw new ArgumentOutOfRangeException(nameof(sortOrder), sortOrder, null)
        };
    }

}