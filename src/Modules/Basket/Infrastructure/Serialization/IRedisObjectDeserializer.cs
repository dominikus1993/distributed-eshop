using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

using Basket.Infrastructure.Model;

using StackExchange.Redis;

namespace Basket.Infrastructure.Serialization;


internal interface IRedisObjectDeserializer
{
    bool Deserialize(RedisValue json, [NotNullWhen(true)]out RedisCustomerBasket? redisCustomerBasket);
    RedisValue Serialize(RedisCustomerBasket obj);
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(RedisCustomerBasket))]
internal partial class RedisJsonContext : JsonSerializerContext
{

}

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