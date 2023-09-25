using Basket.Core.Events;
using Basket.Core.Repositories;
using Basket.Infrastructure.Redis;
using Basket.Infrastructure.Repositories;
using Basket.Infrastructure.Serialization;

using Messaging.Extensions;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Basket.Infrastructure.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddBasketInfrastructure(this WebApplicationBuilder builder)
    {
        var redis = RedisConnectionFactory.Connect(builder.Configuration.GetConnectionString("BasketDb")!);
        builder.Services.AddSingleton(redis);
        builder.Services.AddSingleton<IRedisObjectDeserializer, SystemTextRedisObjectDeserializer>();
        builder.Services.AddTransient<ICustomerBasketReader, RedisCustomerBasketRepository>();
        builder.Services.AddTransient<ICustomerBasketWriter, RedisCustomerBasketRepository>();

        builder.Services.AddRabbitMq(builder.Configuration, configuration =>
            {
                configuration.SetJsonTypeInfoResolver(RabbitMqEventsJsonSerializerContext.Default);
            })
            .AddPublisher<BasketItemWasAdded>()
            .AddPublisher<BasketItemWasRemoved>();
        
        return builder;
    }
}