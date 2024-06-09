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

public class Message1 : IMessage
{
    public string? Message { get; init; }
    public Guid Id { get; set; } = Guid.NewGuid();
    public long Timestamp { get; set; }
    public long ConsumedAtTimestamp { get; set; }
}

public class Message2 : IMessage
{
    public string? Message { get; init; }
    public Guid Id { get; set; } = Guid.NewGuid();
    public long Timestamp { get; set; }
    
    public long ConsumedAtTimestamp { get; set; }
}

[Collection(nameof(RabbitMqFixtureCollectionTest))]
public class RabbitMqmultiMessageConsumerTests
{
    private readonly RabbitMqFixture _rabbitMqFixture;

    public RabbitMqmultiMessageConsumerTests(RabbitMqFixture rabbitMqFixture)
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
    public async Task TestMessagesSubscription(MultiMessageRabbitMqSubscriptionConfiguration config, Message1 message1, Message2 message2, string message1Topic, string message2Topic)
    {
        // Arrange 
        
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));
        var message1Publisher = new RabbitMqMessagePublisher<Message1>(_rabbitMqFixture.Bus.Advanced, new RabbitMqPublisherConfig<Message1>(){Exchange = config.Exchange, Topic = message1Topic });
        var message2Publisher = new RabbitMqMessagePublisher<Message2>(_rabbitMqFixture.Bus.Advanced, new RabbitMqPublisherConfig<Message2>(){ Exchange = config.Exchange, Topic = message2Topic });
        var exchange = await _rabbitMqFixture.Bus.Advanced.DeclareExchangeAsync(config, cancellationToken: cts.Token);
        var configuration = new MultiMessageRabbitMqSubscriptionConfiguration()
        {
            Exchange = config.Exchange, Queue = config.Queue,
        };
        configuration.AddSubscription<Message1, TestSubscriber<Message1>>(new MultiMessageRabbitMqSubscription(){RouteKey = message1Topic});
        configuration.AddSubscription<Message2, TestSubscriber<Message2>>(new MultiMessageRabbitMqSubscription(){RouteKey = message2Topic});
        var consumers = await RabbitMqTestConsumer.CreateMultiMessageConsumerAsync<Message1, Message2>(_rabbitMqFixture.Bus.Advanced,
            configuration, 1, 1, cts.Token);
        await using var consumer1 = consumers.Item1;
        await using var consumer2 = consumers.Item2;
        // Act

        await message1Publisher.Publish(message1, cancellationToken: cts.Token);
        await message2Publisher.Publish(message2, cancellationToken: cts.Token);
        
        var subject1T = consumer1.ConsumeOne();
        var subject2T = consumer2.ConsumeOne();
        
        await Task.WhenAll(subject1T, subject2T);
        
        var subject1 = await subject1T;
        var subject2 = await subject2T;

        subject1.ShouldNotBeNull();
        subject1.Message.ShouldNotBeNullOrEmpty();
        subject1.Message.ShouldBe(message1.Message);
        
        subject2.ShouldNotBeNull();
        subject2.Message.ShouldNotBeNullOrEmpty();
        subject2.Message.ShouldBe(message2.Message);
    }
}