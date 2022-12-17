using Basket.Core.Repositories;
using Basket.Infrastructure.Redis;
using Basket.Infrastructure.Repositories;
using Basket.Infrastructure.Serialization;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Basket.Infrastructure.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static IServiceCollection AddBasketInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var redis = RedisConnectionFactory.Connect(configuration.GetConnectionString("BasketDb")!);
        services.AddSingleton(redis);
        services.AddSingleton<IRedisObjectDeserializer, MemoryPackObjectDeserializer>();
        services.AddTransient<ICustomerBasketReader, RedisCustomerBasketRepository>();
        services.AddTransient<ICustomerBasketWriter, RedisCustomerBasketRepository>();
        return services;
    }
}