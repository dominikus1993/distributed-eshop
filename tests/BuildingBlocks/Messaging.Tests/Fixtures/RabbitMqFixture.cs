using System.Diagnostics.CodeAnalysis;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

using EasyNetQ;

using Xunit;

namespace Messaging.Tests.Fixtures;

public sealed class RabbitMqFixture : IAsyncLifetime, IDisposable
{
    private readonly TestcontainerMessageBrokerConfiguration _rabbitmqConfiguration =
        new RabbitMqTestcontainerConfiguration() { Username = "guest", Password = "guest",  };
    
    public RabbitMqFixture()
    {
        RabbitMq = new TestcontainersBuilder<RabbitMqTestcontainer>()
            .WithMessageBroker(_rabbitmqConfiguration)
            .Build();
        
    }
    
    public TestcontainerMessageBroker RabbitMq { get; private set; }
    public IBus Bus { get; private set; } = null!;

    [MemberNotNull(nameof(Bus))]
    public async Task InitializeAsync()
    {
        await RabbitMq.StartAsync();
        Bus = RabbitHutch.CreateBus(RabbitMq.ConnectionString(), register =>
        {
            register.EnableSystemTextJson();
        });
    }

    public async Task DisposeAsync()
    {
        await RabbitMq.DisposeAsync();
    }

    public void Dispose()
    {
        this._rabbitmqConfiguration.Dispose();
        Bus.Dispose();
    } 
}

[CollectionDefinition(nameof(RabbitMqFixtureCollectionTest))]
public class RabbitMqFixtureCollectionTest : ICollectionFixture<RabbitMqFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
