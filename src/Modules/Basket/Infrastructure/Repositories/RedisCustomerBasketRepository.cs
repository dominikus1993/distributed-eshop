using Basket.Core.Model;
using Basket.Core.Repositories;
using Basket.Infrastructure.Extensions;
using Basket.Infrastructure.Model;
using Basket.Infrastructure.Serialization;

using Common.Types;

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

        var items = model.Items.Select(static item => new Product(new ItemId(item.ItemId), new ItemQuantity(item.Quantity)));

        return CustomerBasket.Active(customerId, [..items]);
    }

    public async Task<Result<UpdateBasketSuccess>> Update(CustomerBasket basket, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var redisBasket = new RedisCustomerBasket(basket);

        var json = _redisObjectDeserializer.Serialize(redisBasket);

        var db = _connectionMultiplexer.GetDatabase();
        
        await db.StringSetAsync(basket.CustomerId.ToRedisKey(), json);

        return Result.Ok(UpdateBasketSuccess.Instance);
    }

    public async Task<Result<RemoveBasketSuccess>> Remove(CustomerId customerId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var key = customerId.ToRedisKey();
        var db = _connectionMultiplexer.GetDatabase();
        await db.KeyDeleteAsync(key);

        return Result.Ok(RemoveBasketSuccess.Instance);
    }
}