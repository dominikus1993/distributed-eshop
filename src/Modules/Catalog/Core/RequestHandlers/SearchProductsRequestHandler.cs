using Catalog.Core.Dto;
using Catalog.Core.Repository;
using Catalog.Core.Requests;

using Mediator;

namespace Catalog.Core.RequestHandlers;

public sealed class SearchProductsRequestHandler : IRequestHandler<SearchProducts, PagedResult<ProductDto>>
{
    private readonly IProductFilter _productFilter;

    public SearchProductsRequestHandler(IProductFilter productFilter)
    {
        _productFilter = productFilter;
    }

    public async ValueTask<PagedResult<ProductDto>> Handle(SearchProducts request, CancellationToken cancellationToken)
    {
        var res = await _productFilter.FilterProducts(
            new Filter()
            {
                PageSize = request.PageSize,
                Page = request.Page,
                Query = request.Query,
                PriceFrom = request.PriceFrom,
                PriceTo = request.PriceTo
            }, cancellationToken);

        if (res.IsEmpty)
        {
            return PagedResult<ProductDto>.Empty;
        }

        var products = res.Data.Select(p => new ProductDto(p));
        return new PagedResult<ProductDto>(products, res.Count, res.Total);
    }
}