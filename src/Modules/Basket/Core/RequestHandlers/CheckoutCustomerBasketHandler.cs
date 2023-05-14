using Basket.Core.Exceptions;
using Basket.Core.Repositories;
using Basket.Core.Requests;

using Mediator;

namespace Basket.Core.RequestHandlers;

public sealed class CheckoutCustomerBasketHandler : IRequestHandler<CheckoutCustomerBasket>
{
    private readonly ICustomerBasketReader _customerBasketReader;
    private readonly ICustomerBasketWriter _customerBasketWriter;

    public CheckoutCustomerBasketHandler(ICustomerBasketReader customerBasketReader, ICustomerBasketWriter customerBasketWriter)
    {
        _customerBasketReader = customerBasketReader;
        _customerBasketWriter = customerBasketWriter;
    }

    public async ValueTask<Unit> Handle(CheckoutCustomerBasket request, CancellationToken cancellationToken)
    {
        var basket = await _customerBasketReader.Find(request.CustomerId, cancellationToken);

        if (basket is not null)
        {
            await _customerBasketWriter.Remove(request.CustomerId, cancellationToken);
            return Unit.Value;
        }

        throw new CustomerBasketNotExistsException(request.CustomerId);
    }
}