using AutoFixture.Xunit2;

using EasyNetQ;
using EasyNetQ.Serialization.SystemTextJson;
using EasyNetQ.Topology;

using Messaging.RabbitMq.Configuration;
using Messaging.RabbitMq.Consumer;
using Messaging.RabbitMq.Extensions;
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
    public long Timestamp { get; set; }
}

[Collection(nameof(RabbitMqFixtureCollectionTest))]
public class RabbitMqConsumerTests
{
    private readonly RabbitMqFixture _rabbitMqFixture;

    public RabbitMqConsumerTests(RabbitMqFixture rabbitMqFixture)
    {
        _rabbitMqFixture = rabbitMqFixture;
    }

    [Theory]
    [InlineAutoData()]
    public async Task TestMessageSubscription(RabbitMqPublisherConfig<Msg> config, Msg msg, string queueName)
    {
        // Arrange 
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));
        var publisher = new RabbitMqMessagePublisher<Msg>(_rabbitMqFixture.Bus.Advanced, config);
        var exchange = await _rabbitMqFixture.Bus.Advanced.DeclareExchangeAsync(config, cancellationToken: cts.Token);
        await using var consumer = await RabbitMqTestConsumer.CreateAsync(_rabbitMqFixture.Bus.Advanced,
            new RabbitMqSubscriptionConfiguration<Msg>()
            {
                Exchange = exchange.Name, Topic = config.Topic, Queue = queueName
            }, 1, cts.Token);
        // Act

        await publisher.Publish(msg, cancellationToken: cts.Token);


        var subject = await consumer.ConsumeOne();

        subject.ShouldNotBeNull();
        subject.Message.ShouldNotBeNullOrEmpty();
        subject.Message.ShouldBe(msg.Message);
    }
    
    [Theory]
    [InlineAutoData()]
    public async Task TestMessagesSubscription(RabbitMqPublisherConfig<Msg> config, Msg[] msgs, string queueName)
    {
        // Arrange 
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));
        var publisher = new RabbitMqMessagePublisher<Msg>(_rabbitMqFixture.Bus.Advanced, config);
        var exchange = await _rabbitMqFixture.Bus.Advanced.DeclareExchangeAsync(config, cancellationToken: cts.Token);
        await using var consumer = await RabbitMqTestConsumer.CreateAsync(_rabbitMqFixture.Bus.Advanced,
            new RabbitMqSubscriptionConfiguration<Msg>()
            {
                Exchange = exchange.Name, Topic = config.Topic, Queue = queueName
            }, msgs.Length, cts.Token);
        // Act

        foreach (var msg in msgs)
        {
            await publisher.Publish(msg, cancellationToken: cts.Token);
        }
        
        var subject = await consumer.Consume().ToListAsync(cancellationToken: cts.Token);

        subject.ShouldNotBeNull();
        subject.ShouldNotBeEmpty();
        subject.Count.ShouldBe(msgs.Length);
        Assert.All(subject, msg =>
        {
            Assert.NotNull(msg);
            Assert.Contains(msgs, x => x.Id == msg.Id);
            Assert.Equivalent(msg.Message, msgs.First(x => x.Id == msg.Id).Message);
        }); // Assert that all messages are received
    }
}