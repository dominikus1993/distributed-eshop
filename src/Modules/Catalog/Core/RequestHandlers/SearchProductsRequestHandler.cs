using Catalog.Core.Dto;
using Catalog.Core.Repository;
using Catalog.Core.Requests;

using Mediator;

namespace Catalog.Core.RequestHandlers;

public sealed class SearchProductsRequestHandler : IStreamRequestHandler<SearchProducts, ProductDto>
{
    private readonly IProductFilter _productFilter;

    public SearchProductsRequestHandler(IProductFilter productFilter)
    {
        _productFilter = productFilter;
    }

    public IAsyncEnumerable<ProductDto> Handle(SearchProducts request, CancellationToken cancellationToken)
    {
        return _productFilter.FilterProducts(
            new Filter()
            {
                PageSize = request.PageSize,
                Page = request.Page,
                Query = request.Query,
                PriceFrom = request.PriceFrom,
                PriceTo = request.PriceTo
            }, cancellationToken).Select(product => new ProductDto(product));
    }
}