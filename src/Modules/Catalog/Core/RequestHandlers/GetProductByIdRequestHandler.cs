using Catalog.Core.Dto;
using Catalog.Core.Repository;
using Catalog.Core.Requests;

using Mediator;

namespace Catalog.Core.RequestHandlers;

public sealed class GetProductByIdRequestHandler : IRequestHandler<GetProductById, ProductDto?>
{
    private readonly IProductReader _productReader;

    public GetProductByIdRequestHandler(IProductReader productReader)
    {
        _productReader = productReader;
    }

    public async ValueTask<ProductDto?> Handle(GetProductById request, CancellationToken cancellationToken)
    {
        var product = await _productReader.GetById(request.ProductId, cancellationToken);
        return product is not null ? new ProductDto(product) : null;
    }
}