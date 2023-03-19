using Basket.Core.Dtos;
using Basket.Core.Model;

using Mediator;

namespace Basket.Core.Requests;

public sealed record GetCustomerBasket(CustomerId CustomerId) : IRequest<CustomerBasketDto?>;

public sealed record AddItemToCustomerBasket(CustomerId CustomerId, Product Item) : IRequest;

public sealed record RemoveItemFromCustomerBasket(CustomerId CustomerId, Product Item) : IRequest;

public sealed record CheckoutCustomerBasket(CustomerId CustomerId) : IRequest;