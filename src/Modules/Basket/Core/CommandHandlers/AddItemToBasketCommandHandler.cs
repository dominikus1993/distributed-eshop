using Basket.Core.Commands;
using Basket.Core.Repositories;

using Mediator;

namespace Basket.Core.CommandHandlers;

public sealed class AddItemToBasketCommandHandler : IRequestHandler<AddItemToBasketCommand>
{
    private readonly ICustomerBasketReader _customerBasketReader;
    private readonly ICustomerBasketWriter _customerBasketWriter;

    public AddItemToBasketCommandHandler(ICustomerBasketReader customerBasketReader, ICustomerBasketWriter customerBasketWriter)
    {
        _customerBasketReader = customerBasketReader;
        _customerBasketWriter = customerBasketWriter;
    }

    public ValueTask<Unit> Handle(AddItemToBasketCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}