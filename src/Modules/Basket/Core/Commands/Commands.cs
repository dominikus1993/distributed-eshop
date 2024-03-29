using Basket.Core.Model;

using Mediator;

namespace Basket.Core.Commands;

public sealed record AddItemToBasketCommand(CustomerId CustomerId, Product Item) : IRequest;