using AutoFixture.Xunit2;

using EasyNetQ;

using Messaging.RabbitMq.Configuration;
using Messaging.RabbitMq.Extensions;
using Messaging.RabbitMq.Publisher;
using Messaging.Tests.Extensions;
using Messaging.Tests.Fixtures;

using Shouldly;

using Xunit;

namespace Messaging.Tests.Subscriber;

[Collection(nameof(RabbitMqFixtureCollectionTest))]
public class RabbitMqMessagePublisherTests 
{
    private readonly RabbitMqFixture _rabbitMqFixture;

    public RabbitMqMessagePublisherTests(RabbitMqFixture rabbitMqFixture)
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
        var queue = await _rabbitMqFixture.Bus.Advanced.QueueDeclareAsync(queueName, cancellationToken: cts.Token);

        await _rabbitMqFixture.Bus.Advanced.BindAsync(exchange, queue, config.Topic, cancellationToken: cts.Token);
        
        // Act
        
        await publisher.Publish(msg, cancellationToken: cts.Token);

        var subject = await _rabbitMqFixture.Bus.Advanced.ConsumeEnumerable<Msg>(queue, cts.Token).FirstOrDefaultAsync(cancellationToken: cts.Token);

        // Assert
        subject.ReceivedInfo.Exchange.ShouldBe(exchange.Name);
        subject.ReceivedInfo.Queue.ShouldBe(queue.Name);
        subject.ReceivedInfo.RoutingKey.ShouldBe(config.Topic);
        
        subject.Message.Body.Message.ShouldBe(msg.Message);
    }
    
    [Theory]
    [InlineAutoData()]
    public async Task TestPublishingNullMessage(RabbitMqPublisherConfig<Msg> config, string queueName)
    {
        // Arrange 
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));
        var publisher = new RabbitMqMessagePublisher<Msg>(_rabbitMqFixture.Bus.Advanced, config);
        var exchange = await _rabbitMqFixture.Bus.Advanced.DeclareExchangeAsync(config, cancellationToken: cts.Token);
        var queue = await _rabbitMqFixture.Bus.Advanced.QueueDeclareAsync(queueName, cancellationToken: cts.Token);

        await _rabbitMqFixture.Bus.Advanced.BindAsync(exchange, queue, config.Topic, cancellationToken: cts.Token);
        
        // Act
        
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await publisher.Publish(null!, cancellationToken: cts.Token));
    }
}