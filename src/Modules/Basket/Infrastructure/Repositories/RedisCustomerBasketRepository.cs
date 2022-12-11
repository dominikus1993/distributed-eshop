using Basket.Core.Model;
using Basket.Core.Repositories;

namespace Basket.Infrastructure.Repositories;

internal sealed class RedisCustomerBasketRepository : ICustomerBasketReader, ICustomerBasketWriter
{
    public Task<CustomerBasket?> Find(CustomerId customerId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<UpdateCustomerBasketResult> Update(CustomerBasket basket, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}