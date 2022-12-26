using EasyNetQ;

using Messaging.Tests.Fixtures;

using Xunit;

using IMessage = Messaging.Abstraction.IMessage;

namespace Messaging.Tests.Subscriber;

public class Msg : IMessage
{
    public string Message { get; init; }
    public Guid Id { get; init; } = Guid.NewGuid();
}

public class RabbitMqMessageConsumerTests : IClassFixture<RabbitMqFixture>
{
    private RabbitMqFixture _rabbitMqFixture;

    public RabbitMqMessageConsumerTests(RabbitMqFixture rabbitMqFixture)
    {
        _rabbitMqFixture = rabbitMqFixture;
    }

    [Fact]
    public async Task TestSubscription()
    {
        // Arrange 
        var msg = new Msg() { Message = "Elo" };

    }
}