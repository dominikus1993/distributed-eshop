using Basket.Core.Events;
using Basket.Core.Model;
using Basket.Core.Repositories;
using Basket.Core.Requests;

using Mediator;

using Messaging.Abstraction;

namespace Basket.Core.RequestHandlers;

public class RemoveItemFromCustomerBasketRequestHandler : IRequestHandler<RemoveItemFromCustomerBasket>
{
    
    private readonly ICustomerBasketReader _customerBasketReader;
    private readonly ICustomerBasketWriter _customerBasketWriter;
    private readonly IMessagePublisher<BasketItemWasRemoved> _messagePublisher;

    public RemoveItemFromCustomerBasketRequestHandler(ICustomerBasketReader customerBasketReader, ICustomerBasketWriter customerBasketWriter, IMessagePublisher<BasketItemWasRemoved> messagePublisher)
    {
        _customerBasketReader = customerBasketReader;
        _customerBasketWriter = customerBasketWriter;
        _messagePublisher = messagePublisher;
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
        }
        else
        {
            await _customerBasketWriter.Update(basket, cancellationToken);
        }

        await _messagePublisher.Publish(new BasketItemWasRemoved(request.CustomerId, request.Item), cancellationToken: cancellationToken);
        return Unit.Value;
    }
}