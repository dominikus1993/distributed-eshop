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
    private readonly TimeProvider _timeProvider;
    
    public RemoveItemFromCustomerBasketRequestHandler(ICustomerBasketReader customerBasketReader, ICustomerBasketWriter customerBasketWriter, IMessagePublisher<BasketItemWasRemoved> messagePublisher, TimeProvider timeProvider)
    {
        _customerBasketReader = customerBasketReader;
        _customerBasketWriter = customerBasketWriter;
        _messagePublisher = messagePublisher;
        _timeProvider = timeProvider;
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
            var result = await _customerBasketWriter.Remove(request.CustomerId, cancellationToken);
            if (!result.IsSuccess)
            {
                throw new InvalidOperationException("can't update basket", result.ErrorValue);
            }
        }
        else
        {
            await _customerBasketWriter.Update(basket, cancellationToken);
        }

        await _messagePublisher.Publish(new BasketItemWasRemoved(request.CustomerId, request.Item, _timeProvider), cancellationToken: cancellationToken);
        return Unit.Value;
    }
}