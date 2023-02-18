using Basket.Core.Model;
using Basket.Core.Repositories;
using Basket.Infrastructure.Extensions;
using Basket.Infrastructure.Model;
using Basket.Infrastructure.Serialization;

using StackExchange.Redis;

namespace Basket.Infrastructure.Repositories;

internal sealed class RedisCustomerBasketRepository : ICustomerBasketReader, ICustomerBasketWriter
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IRedisObjectDeserializer _redisObjectDeserializer;
    public RedisCustomerBasketRepository(IConnectionMultiplexer connectionMultiplexer, IRedisObjectDeserializer redisObjectDeserializer)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _redisObjectDeserializer = redisObjectDeserializer;
    }

    public async Task<CustomerBasket?> Find(CustomerId customerId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var db = _connectionMultiplexer.GetDatabase();
        var result = await db.StringGetAsync(customerId.ToRedisKey());
        if (!_redisObjectDeserializer.Deserialize(result, out var model))
        {
            return null;
        }

        var items = model.Items.Select(item => new BasketItem(new ItemId(item.ItemId), new ItemQuantity(item.Quantity)))
            .ToArray();

        return CustomerBasket.Empty(customerId).AddItems(items);
    }

    public async Task<UpdateCustomerBasketResult> Update(CustomerBasket basket, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var redisBasket = new RedisCustomerBasket(basket);

        var json = _redisObjectDeserializer.Serialize(redisBasket);

        var db = _connectionMultiplexer.GetDatabase();
        await db.StringSetAsync(basket.CustomerId.ToRedisKey(), json);

        return UpdateBasketSuccess.Instance;
    }

    public async Task<RemoveCustomerBasketResult> Remove(CustomerId customerId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var key = customerId.ToRedisKey();
        var db = _connectionMultiplexer.GetDatabase();
        await db.KeyDeleteAsync(key);

        return RemoveBasketSuccess.Instance;
    }
}