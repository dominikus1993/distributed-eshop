using Basket.Infrastructure.Redis;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

using StackExchange.Redis;

namespace Basket.Tests.Fixture;

public class RedisFixture : IAsyncLifetime, IDisposable
{
    private readonly TestcontainerDatabaseConfiguration _configuration = new RedisTestcontainerConfiguration("redis:6-alpine");

    public RedisFixture()
    {
        Redis = new TestcontainersBuilder<RedisTestcontainer>()
            .WithDatabase(this._configuration)
            .Build();
    }
    
    public TestcontainerDatabase Redis { get; private set; }
    public IConnectionMultiplexer RedisConnection { get; private set; }

    public async Task InitializeAsync()
    {
        await Redis.StartAsync();
        RedisConnection = RedisConnectionFactory.Connect(Redis.ConnectionString);
    }

    public async Task DisposeAsync()
    {
        await RedisConnection.DisposeAsync();
        await Redis.DisposeAsync();
    }

    public void Dispose()
    {
        this._configuration.Dispose();
    }
}