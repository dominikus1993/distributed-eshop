using Basket.Model;

using Mediator;

namespace Basket.Commands;

public sealed record AddItemToBasketCommand(CustomerId CustomerId, BasketItem Item) : IRequest;