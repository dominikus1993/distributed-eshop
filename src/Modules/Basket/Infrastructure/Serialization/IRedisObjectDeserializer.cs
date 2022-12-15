using System.Text.Json;
using System.Text.Json.Serialization;

using Basket.Infrastructure.Model;

namespace Basket.Infrastructure.Serialization;


internal interface IRedisObjectDeserializer
{
    RedisCustomerBasket? Deserialize(string json);
    string Serialize(RedisCustomerBasket obj);
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(RedisCustomerBasket))]
internal partial class RedisJsonContext : JsonSerializerContext
{

}

internal sealed class SystemTextRedisObjectDeserializer : IRedisObjectDeserializer
{
    public RedisCustomerBasket? Deserialize(string json)
    {
        return JsonSerializer.Deserialize(json, RedisJsonContext.Default.RedisCustomerBasket);
    }

    public string Serialize(RedisCustomerBasket obj)
    {
        return JsonSerializer.Serialize(obj, RedisJsonContext.Default.RedisCustomerBasket);
    }
}