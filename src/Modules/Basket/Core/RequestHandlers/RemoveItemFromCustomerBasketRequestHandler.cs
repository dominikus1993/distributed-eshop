using Basket.Core.Model;
using Basket.Core.Repositories;
using Basket.Core.Requests;

using Mediator;

namespace Basket.Core.RequestHandlers;

public class RemoveItemFromCustomerBasketRequestHandler : IRequestHandler<RemoveItemFromCustomerBasket>
{
    
    private readonly ICustomerBasketReader _customerBasketReader;
    private readonly ICustomerBasketWriter _customerBasketWriter;

    public RemoveItemFromCustomerBasketRequestHandler(ICustomerBasketReader customerBasketReader, ICustomerBasketWriter customerBasketWriter)
    {
        _customerBasketReader = customerBasketReader;
        _customerBasketWriter = customerBasketWriter;
    }

    public async ValueTask<Unit> Handle(RemoveItemFromCustomerBasket request, CancellationToken cancellationToken)
    {
        var basket = await _customerBasketReader.Find(request.CustomerId, cancellationToken);

        if (basket is null or { IsEmpty: true })
        {
            return Unit.Value;
        }

        basket = basket.RemoveItem(request.Item);
        
        if (basket.IsEmpty)
        {
            await _customerBasketWriter.Remove(request.CustomerId, cancellationToken);
            return Unit.Value;
        }
        
        await _customerBasketWriter.Update(basket, cancellationToken);
        
        return Unit.Value;
    }
}