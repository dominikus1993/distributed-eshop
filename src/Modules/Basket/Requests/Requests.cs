using Basket.Model;

using Mediator;

namespace Basket.Requests;

public sealed record GetCustomerBasket(CustomerId CustomerId) : IRequest<CustomerBasket?>;