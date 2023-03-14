
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

using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace Basket.Tests.Fixture;

public sealed class BasketApiFixture : IAsyncLifetime, IDisposable
{
    
    public BasketApiFixture()
    {
        Redis = new RedisBuilder().Build();
        RabbitMq = new RabbitMqBuilder().Build();
    }
    
    public RedisContainer Redis { get; private set; }
    public RabbitMqContainer RabbitMq { get; private set; }
    public IConnectionMultiplexer RedisConnection { get; private set; } = null!;


    public async Task<IAlbaHost> GetHost(JwtSecurityStub stub)
    {
        return await AlbaHost.For<Program>(h =>
        {
            h.UseSetting("ConnectionStrings:BasketDb", Redis.GetConnectionString());
            h.UseSetting("RabbitMq:Connection", RabbitMq.ConnectionString());
            h.ConfigureAppConfiguration((_, builder) =>
            {
                builder.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("./Api/appsettings.json", optional: false, reloadOnChange: true);
                var dict = new Dictionary<string, string>
                {
                    { "ConnectionStrings:BasketDb", Redis.GetConnectionString()! },
                };
                builder.AddInMemoryCollection(dict!);
            });
        }, stub);
    }
    
    public async Task InitializeAsync()
    {
        await RabbitMq.StartAsync();
        await Redis.StartAsync();
        RedisConnection = RedisConnectionFactory.Connect(Redis.GetConnectionString());
    }

    public async Task DisposeAsync()
    {
        await RedisConnection.DisposeAsync();
        await Redis.DisposeAsync();
        await RabbitMq.DisposeAsync();
    }

    public void Dispose()
    {
    } 
}

[CollectionDefinition(nameof(BasketApiFixtureCollectionTest))]
public class BasketApiFixtureCollectionTest : ICollectionFixture<BasketApiFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
