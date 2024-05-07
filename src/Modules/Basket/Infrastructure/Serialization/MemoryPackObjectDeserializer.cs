using System.Diagnostics.CodeAnalysis;

using Basket.Infrastructure.Model;

using MemoryPack;

using StackExchange.Redis;

namespace Basket.Infrastructure.Serialization;

internal sealed class MemoryPackObjectDeserializer : IRedisObjectDeserializer
{
    public bool Deserialize(RedisValue json, [NotNullWhen(true)]out RedisCustomerBasket? redisCustomerBasket)
    {
        if (json.IsNullOrEmpty)
        {
            redisCustomerBasket = null;
            return false;
        }
        ReadOnlyMemory<byte> memory = json;
        var result = MemoryPackSerializer.Deserialize<RedisCustomerBasket>(memory.Span);
        if (result is null)
        {
            redisCustomerBasket = result;
            return false;
        }
        redisCustomerBasket = result!;
        return true;
    }

    public RedisValue Serialize(RedisCustomerBasket obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return MemoryPackSerializer.Serialize(obj);
    }
}