using Basket.Core.Events;
using Basket.Core.Model;
using Basket.Core.Repositories;
using Basket.Core.Requests;

using Mediator;

using Messaging.Abstraction;

namespace Basket.Core.RequestHandlers;

public sealed class AddItemToCustomerBasketHandler : IRequestHandler<AddItemToCustomerBasket>
{
    private readonly ICustomerBasketReader _customerBasketReader;
    private readonly ICustomerBasketWriter _customerBasketWriter;
    private readonly IMessagePublisher<BasketItemWasAdded> _messagePublisher;
    private readonly TimeProvider _timeProvider;

    public AddItemToCustomerBasketHandler(ICustomerBasketReader customerBasketReader, ICustomerBasketWriter customerBasketWriter, IMessagePublisher<BasketItemWasAdded> messagePublisher, TimeProvider timeProvider)
    {
        _customerBasketReader = customerBasketReader;
        _customerBasketWriter = customerBasketWriter;
        _messagePublisher = messagePublisher;
        _timeProvider = timeProvider;
    }

    public async ValueTask<Unit> Handle(AddItemToCustomerBasket request, CancellationToken cancellationToken)
    {
        var basket = await _customerBasketReader.Find(request.CustomerId, cancellationToken);
        basket ??= CustomerBasket.Empty(request.CustomerId);

        basket = basket.AddItem(request.Item);

        var result = await _customerBasketWriter.Update(basket, cancellationToken);
        if (!result.IsSuccess)
        {
            throw new InvalidOperationException("can't update basket", result.ErrorValue);
        }
        
        var res = await _messagePublisher.Publish(new BasketItemWasAdded(request.CustomerId, request.Item, _timeProvider), cancellationToken: cancellationToken);
        if (!res.IsSuccess)
        {
            throw new InvalidOperationException("can't publish message", res.ErrorValue);
        }
        return Unit.Value;
    }
}