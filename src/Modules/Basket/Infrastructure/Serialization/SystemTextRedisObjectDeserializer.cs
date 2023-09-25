using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

using Basket.Infrastructure.Model;

using StackExchange.Redis;

namespace Basket.Infrastructure.Serialization;

internal sealed class SystemTextRedisObjectDeserializer : IRedisObjectDeserializer
{
    public bool Deserialize(RedisValue json, [NotNullWhen(true)]out RedisCustomerBasket? redisCustomerBasket)
    {
        if (json.IsNullOrEmpty)
        {
            redisCustomerBasket = null;
            return false;
        }
        ReadOnlyMemory<byte> memory = json;
        var result = JsonSerializer.Deserialize(memory.Span, RedisJsonContext.Default.RedisCustomerBasket);
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
        return JsonSerializer.SerializeToUtf8Bytes(obj, RedisJsonContext.Default.RedisCustomerBasket);
    }
}