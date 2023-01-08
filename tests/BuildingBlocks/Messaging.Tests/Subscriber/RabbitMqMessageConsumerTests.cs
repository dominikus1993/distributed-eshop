using EasyNetQ;
using EasyNetQ.Serialization.SystemTextJson;
using EasyNetQ.Topology;

using Messaging.Extensions;
using Messaging.RabbitMq.Configuration;
using Messaging.RabbitMq.Consumer;
using Messaging.RabbitMq.Publisher;
using Messaging.Tests.Extensions;
using Messaging.Tests.Fixtures;

using Shouldly;

using Xunit;

using IMessage = Messaging.Abstraction.IMessage;

namespace Messaging.Tests.Subscriber;

public class Msg : IMessage
{
    public string? Message { get; init; }
    public Guid Id { get; set; } = Guid.NewGuid();
}

[Collection(nameof(RabbitMqFixtureCollection))]
public class RabbitMqConsumerTests
{
    private readonly RabbitMqFixture _rabbitMqFixture;

    public RabbitMqConsumerTests(RabbitMqFixture rabbitMqFixture)
    {
        _rabbitMqFixture = rabbitMqFixture;
    }

    [Fact]
    public async Task TestMessageSubscription()
    {
        // Arrange 
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));
        var config = new RabbitMqPublisherConfig<Msg>() { Exchange = "xDD", Topic = "test" };
        var msg = new Msg() { Message = "Elo" };
        var publisher = new RabbitMqMessagePublisher<Msg>(_rabbitMqFixture.Bus.Advanced, config);
        var exchange = await _rabbitMqFixture.Bus.Advanced.DeclareExchangeAsync(config, cancellationToken: cts.Token);
        await using var consumer = await RabbitMqTestConsumer.CreateAsync(_rabbitMqFixture.Bus.Advanced,
            new RabbitMqSubscriptionConfiguration<Msg>()
            {
                Exchange = exchange.Name, Topic = config.Topic, Queue = $"test-{Guid.NewGuid()}"
            }, 1, cts.Token);
        // Act

        await publisher.Publish(msg, cancellationToken: cts.Token);


        var subject = await consumer.ConsumeOne();

        subject.ShouldNotBeNull();
        subject.Message.ShouldNotBeNullOrEmpty();
        subject.Message.ShouldBe(msg.Message);
    }
}