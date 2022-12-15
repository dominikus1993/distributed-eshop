using StackExchange.Redis;

namespace Basket.Infrastructure.Redis;

internal static class RedisConnectionFactory
{
    public static IConnectionMultiplexer Connect(string connectionString)
    {
        var config = ConfigurationOptions.Parse(connectionString);
        config.AbortOnConnectFail = false;
        return ConnectionMultiplexer.Connect(config);
    }
}