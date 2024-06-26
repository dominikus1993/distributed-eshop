using System.Diagnostics.CodeAnalysis;

using Common.Types;

using EasyNetQ;
using EasyNetQ.Topology;

using Messaging.Abstraction;
using Messaging.RabbitMq.Configuration;
using Messaging.RabbitMq.Telemetry;

using IMessage = Messaging.Abstraction.IMessage;

namespace Messaging.RabbitMq.Publisher;


internal sealed class Message<T> : IMessage<T>
{
    private static readonly Type CachedType = typeof(T);
    public MessageProperties? Properties { get; }
    public Type MessageType { get; }
    public T Body { get; }

    public object GetBody() { return Body!; }
    
    private Message(T body, MessageProperties properties, Type messageType)
    {
        Body = body;
        Properties = properties;
        MessageType = messageType;
    }

    public static Message<T> Create(T body, MessageProperties properties)
    {
        ArgumentNullException.ThrowIfNull(properties);
        ArgumentNullException.ThrowIfNull(body);
        return new Message<T>(body, properties, CachedType);
    }
}

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

    public async Task<Result<Unit>> Publish([NotNull] T message, IMessageContext? ctx = null, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(message);
            await PublishInternal(message, ctx, cancellationToken);
            return Result.UnitResult;
        }
        catch (Exception e)
        {
            return Result.Failure<Unit>(e);
        }
    }

    private async Task PublishInternal(T message, IMessageContext? ctx = null,
        CancellationToken cancellationToken = default)
    {
        var exchange = await _advancedBus.ExchangeDeclareAsync(_publisherConfig.Exchange, ExchangeType.Topic, true, false,
            cancellationToken);
        using var activity = RabbitMqTelemetry.RabbitMqActivitySource.Start(activityContext: ctx?.Context);
        MessageProperties messageProps = GetMessageProps(message, _publisherConfig);
        if (activity is not null)
        {
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
        await _advancedBus.PublishAsync(exchange, _publisherConfig.Topic, false, Message<T>.Create(message, messageProps), cancellationToken);
    }

    private static MessageProperties GetMessageProps(T message, RabbitMqPublisherConfig<T> config)
    {
        var props =  new MessageProperties()
        {
            Type = MessageName,
            ContentType = "application/json",
            Timestamp = message.Timestamp,
            MessageId = message.Id.ToString()
        };
        
        if (config.TTL.HasValue)
        {
            props.Expiration = config.TTL.Value;
        }
        
        return props;
    }
    
    
}