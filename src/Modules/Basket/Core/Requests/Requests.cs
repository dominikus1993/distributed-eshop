using Basket.Core.Dtos;
using Basket.Core.Model;

using Mediator;

namespace Basket.Core.Requests;

public sealed record GetCustomerBasket(CustomerId CustomerId) : IRequest<CustomerBasketDto?>;

public sealed record AddItemToCustomerBasket(CustomerId CustomerId, BasketItem Item) : IRequest;