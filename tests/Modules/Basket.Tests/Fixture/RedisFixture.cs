using Basket.Core.Repositories;
using Basket.Infrastructure.Redis;
using Basket.Infrastructure.Repositories;
using Basket.Infrastructure.Serialization;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

using StackExchange.Redis;

using Testcontainers.Redis;

namespace Basket.Tests.Fixture;

public sealed class RedisFixture : IAsyncLifetime, IDisposable
{
    public RedisFixture()
    {
        Redis = new RedisBuilder().Build();
    }
    
    public RedisContainer Redis { get; private set; }
    public IConnectionMultiplexer RedisConnection { get; private set; }
    public ICustomerBasketReader CustomerBasketReader { get; private set; }
    public ICustomerBasketWriter CustomerBasketWriter { get; private set; }

    public async Task InitializeAsync()
    {
        await Redis.StartAsync();
        RedisConnection = RedisConnectionFactory.Connect(Redis.GetConnectionString());
        CustomerBasketReader = new RedisCustomerBasketRepository(RedisConnection, new SystemTextRedisObjectDeserializer());
        CustomerBasketWriter = new RedisCustomerBasketRepository(RedisConnection, new SystemTextRedisObjectDeserializer());
    }

    public async Task DisposeAsync()
    {
        await RedisConnection.DisposeAsync();
        await Redis.DisposeAsync();
    }

    public void Dispose()
    {
    }
}


[CollectionDefinition(nameof(RedisFixtureCollectionTest))]
public class RedisFixtureCollectionTest : ICollectionFixture<RedisFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
