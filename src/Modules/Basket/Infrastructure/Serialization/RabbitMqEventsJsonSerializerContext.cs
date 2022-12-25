using System.Text.Json.Serialization;

using Basket.Core.Events;

namespace Basket.Infrastructure.Serialization;

[JsonSerializable(typeof(BasketItemWasAdded))]
[JsonSerializable(typeof(BasketItemWasRemoved))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
internal partial class RabbitMqEventsJsonSerializerContext : JsonSerializerContext
{
    
}