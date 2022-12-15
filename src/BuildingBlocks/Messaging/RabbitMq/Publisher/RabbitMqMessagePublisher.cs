using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using EasyNetQ;
using EasyNetQ.Topology;

using Messaging.Abstraction;
using Messaging.RabbitMq.Configuration;
using Messaging.RabbitMq.Telemetry;

using IMessage = Messaging.Abstraction.IMessage;

namespace Messaging.RabbitMq.Publisher;

internal sealed class RabbitMqMessagePublisher<T> : IMessagePublisher<T> where T : IMessage
{
    
    private static readonly string MessageName = typeof(T).FullName!;
    private readonly IAdvancedBus _advancedBus;
    private readonly RabbitMqPublisherConfig<T> _publisherConfig;

    public RabbitMqMessagePublisher(IAdvancedBus advancedBus, RabbitMqPublisherConfig<T> publisherConfig)
    {
        _advancedBus = advancedBus;
        _publisherConfig = publisherConfig;
    }

    public async Task Publish([NotNull] T message, IMessageContext ctx, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);
        var exchange = await _advancedBus.ExchangeDeclareAsync(_publisherConfig.Exchange, ExchangeType.Topic, true, false,
            cancellationToken);
        using var activity = RabbitMqTelemetry.RabbitMqActivitySource.StartActivity("rabbitmq.publish", ActivityKind.Producer);
        MessageProperties? messageProps = null;
        if (activity is not null)
        {
            messageProps = new MessageProperties();
            activity.SetTag("messaging.rabbitmq.routing_key", _publisherConfig.Topic);
            activity.SetTag("messaging.destination", _publisherConfig.Exchange);
            activity.SetTag("messaging.system", "rabbitmq");
            activity.SetTag("messaging.destination_kind", "topic");
            activity.SetTag("messaging.protocol", "AMQP");
            activity.SetTag("messaging.protocol_version", "0.9.1");
            activity.SetTag("messaging.method", "bus");
            activity.SetTag("messaging.message_name", MessageName);
            RabbitMqTelemetry.AddActivityToHeader(activity, messageProps);
        }
        await _advancedBus.PublishAsync(exchange, _publisherConfig.Topic, false, new Message<T>(message, messageProps), cancellationToken);
    }
}