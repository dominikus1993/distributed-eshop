using EasyNetQ;
using EasyNetQ.Serialization.SystemTextJson;
using EasyNetQ.Topology;

using Messaging.Extensions;
using Messaging.RabbitMq.Configuration;
using Messaging.RabbitMq.Publisher;
using Messaging.Tests.Extensions;
using Messaging.Tests.Fixtures;

using Shouldly;

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
    private readonly RabbitMqFixture _rabbitMqFixture;

    public RabbitMqMessageConsumerTests(RabbitMqFixture rabbitMqFixture)
    {
        _rabbitMqFixture = rabbitMqFixture;
    }

    [Fact]
    public async Task TestPublishingMessage()
    {
        // Arrange 
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));
        var config = new RabbitMqPublisherConfig<Msg>() { Exchange = "xDD", Topic = "test" };
        var msg = new Msg() { Message = "Elo" };
        var publisher = new RabbitMqMessagePublisher<Msg>(_rabbitMqFixture.Bus.Advanced, config);
        var exchange = await _rabbitMqFixture.Bus.Advanced.DeclareExchangeAsync(config, cancellationToken: cts.Token);
        var queue = await _rabbitMqFixture.Bus.Advanced.QueueDeclareAsync($"test-{Guid.NewGuid()}", cancellationToken: cts.Token);

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
    
    [Fact]
    public async Task TestPublishingNullMessage()
    {
        // Arrange 
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(30));
        var config = new RabbitMqPublisherConfig<Msg>() { Exchange = Guid.NewGuid().ToString(), Topic = "test" };
        var publisher = new RabbitMqMessagePublisher<Msg>(_rabbitMqFixture.Bus.Advanced, config);
        var exchange = await _rabbitMqFixture.Bus.Advanced.DeclareExchangeAsync(config, cancellationToken: cts.Token);
        var queue = await _rabbitMqFixture.Bus.Advanced.QueueDeclareAsync($"test-{Guid.NewGuid()}", cancellationToken: cts.Token);

        await _rabbitMqFixture.Bus.Advanced.BindAsync(exchange, queue, config.Topic, cancellationToken: cts.Token);
        
        // Act
        
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await publisher.Publish(null!, cancellationToken: cts.Token));
    }
}