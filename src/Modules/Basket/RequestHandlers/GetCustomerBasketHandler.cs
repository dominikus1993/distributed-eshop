using Basket.Model;
using Basket.Requests;

using Mediator;

namespace Basket.RequestHandlers;

public sealed class GetCustomerBasketHandler : IRequestHandler<GetCustomerBasket, CustomerBasket?>
{
    public ValueTask<CustomerBasket?> Handle(GetCustomerBasket request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}