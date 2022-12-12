
using Alba;
using Alba.Security;

using Basket.Infrastructure.Redis;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

using Microsoft.Extensions.Configuration;

using StackExchange.Redis;

namespace Basket.Tests.Fixture;

public class BasketApiFixture : IAsyncLifetime, IDisposable
{
    private readonly TestcontainerDatabaseConfiguration _configuration = new RedisTestcontainerConfiguration("redis:6-alpine");

    public BasketApiFixture()
    {
        Redis = new TestcontainersBuilder<RedisTestcontainer>()
            .WithDatabase(this._configuration)
            .Build();
    }
    
    public TestcontainerDatabase Redis { get; private set; }
    public IConnectionMultiplexer RedisConnection { get; private set; }


    public async Task<IAlbaHost> GetHost(JwtSecurityStub stub)
    {
        return await AlbaHost.For<Program>(host =>
        {
            host.ConfigureAppConfiguration((ctx, builder) =>
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