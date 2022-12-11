using Basket.Model;
using Basket.Repositories;
using Basket.Requests;

using Mediator;

namespace Basket.RequestHandlers;

public sealed class GetCustomerBasketHandler : IRequestHandler<GetCustomerBasket, CustomerBasket?>
{
    private ICustomerBasketReader _customerBasketReader;

    public GetCustomerBasketHandler(ICustomerBasketReader customerBasketReader)
    {
        _customerBasketReader = customerBasketReader;
    }

    public ValueTask<CustomerBasket?> Handle(GetCustomerBasket request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}