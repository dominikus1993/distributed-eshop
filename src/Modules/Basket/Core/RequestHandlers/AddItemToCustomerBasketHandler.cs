using Basket.Core.Model;
using Basket.Core.Repositories;
using Basket.Core.Requests;

using Mediator;

namespace Basket.Core.RequestHandlers;

public sealed class AddItemToCustomerBasketHandler : IRequestHandler<AddItemToCustomerBasket>
{
    private readonly ICustomerBasketReader _customerBasketReader;
    private readonly ICustomerBasketWriter _customerBasketWriter;

    public AddItemToCustomerBasketHandler(ICustomerBasketReader customerBasketReader, ICustomerBasketWriter customerBasketWriter)
    {
        _customerBasketReader = customerBasketReader;
        _customerBasketWriter = customerBasketWriter;
    }

    public async ValueTask<Unit> Handle(AddItemToCustomerBasket request, CancellationToken cancellationToken)
    {
        var basket = await _customerBasketReader.Find(request.CustomerId, cancellationToken);
        basket ??= CustomerBasket.Empty(request.CustomerId);

        basket = basket.AddItem(request.Item);

        await _customerBasketWriter.Update(basket, cancellationToken);
        
        return Unit.Value;
    }
}