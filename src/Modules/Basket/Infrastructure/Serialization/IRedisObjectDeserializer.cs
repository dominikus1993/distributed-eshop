using System.Diagnostics.CodeAnalysis;
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
internal sealed partial class RedisJsonContext : JsonSerializerContext
{

}