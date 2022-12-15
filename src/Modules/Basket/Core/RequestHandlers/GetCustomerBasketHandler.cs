using Basket.Core.Dtos;
using Basket.Core.Model;
using Basket.Core.Repositories;
using Basket.Core.Requests;

using Mediator;

namespace Basket.Core.RequestHandlers;

public sealed class GetCustomerBasketHandler : IRequestHandler<GetCustomerBasket, CustomerBasketDto?>
{
    private readonly ICustomerBasketReader _customerBasketReader;

    public GetCustomerBasketHandler(ICustomerBasketReader customerBasketReader)
    {
        _customerBasketReader = customerBasketReader;
    }

    public async ValueTask<CustomerBasketDto?> Handle(GetCustomerBasket request, CancellationToken cancellationToken)
    {
        var result = await _customerBasketReader.Find(request.CustomerId, cancellationToken);
        return result is null ? null : new CustomerBasketDto(result);
    }
}