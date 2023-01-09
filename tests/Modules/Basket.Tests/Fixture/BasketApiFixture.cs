
using Alba;
using Alba.Security;

using Basket.Infrastructure.Redis;
using Basket.Tests.Infrastructure.Extensions;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using StackExchange.Redis;

namespace Basket.Tests.Fixture;

public sealed class BasketApiFixture : IAsyncLifetime, IDisposable
{
    private readonly TestcontainerDatabaseConfiguration _configuration = new RedisTestcontainerConfiguration("redis:6-alpine");

    private readonly TestcontainerMessageBrokerConfiguration _rabbitmqConfiguration =
        new RabbitMqTestcontainerConfiguration() { Username = "guest", Password = "guest", };
    
    public BasketApiFixture()
    {
        Redis = new TestcontainersBuilder<RedisTestcontainer>()
            .WithDatabase(this._configuration)
            .Build();
        RabbitMq = new TestcontainersBuilder<RabbitMqTestcontainer>()
            .WithMessageBroker(_rabbitmqConfiguration)
            .Build();
    }
    
    public TestcontainerDatabase Redis { get; private set; }
    public TestcontainerMessageBroker RabbitMq { get; private set; }
    public IConnectionMultiplexer RedisConnection { get; private set; } = null!;


    public async Task<IAlbaHost> GetHost(JwtSecurityStub stub)
    {
        return await AlbaHost.For<Program>(h =>
        {
            h.UseSetting("ConnectionStrings:BasketDb", Redis.ConnectionString);
            h.UseSetting("RabbitMq:Connection", RabbitMq.ConnectionString());
            h.ConfigureAppConfiguration((_, builder) =>
            {
                builder.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("./Api/appsettings.json", optional: false, reloadOnChange: true);
                var dict = new Dictionary<string, string>
                {
                    { "ConnectionStrings:BasketDb", Redis.ConnectionString! },
                };
                builder.AddInMemoryCollection(dict!);
            });
        }, stub);
    }
    
    public async Task InitializeAsync()
    {
        await RabbitMq.StartAsync();
        await Redis.StartAsync();
        RedisConnection = RedisConnectionFactory.Connect(Redis.ConnectionString);
    }

    public async Task DisposeAsync()
    {
        await RedisConnection.DisposeAsync();
        await Redis.DisposeAsync();
        await RabbitMq.DisposeAsync();
    }

    public void Dispose()
    {
        this._configuration.Dispose();
        this._rabbitmqConfiguration.Dispose();
    } 
}

[CollectionDefinition(nameof(BasketApiFixtureCollectionTest))]
public class BasketApiFixtureCollectionTest : ICollectionFixture<RedisFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
