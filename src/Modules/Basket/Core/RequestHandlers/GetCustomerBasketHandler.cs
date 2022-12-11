using Basket.Core.Model;
using Basket.Core.Repositories;
using Basket.Core.Requests;

using Mediator;

namespace Basket.Core.RequestHandlers;

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