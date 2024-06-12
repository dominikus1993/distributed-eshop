using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

using Common.Types;

using EasyNetQ;
using EasyNetQ.Consumer;
using EasyNetQ.Topology;

using Messaging.Abstraction;
using Messaging.Logging;
using Messaging.RabbitMq.Configuration;
using Messaging.RabbitMq.Telemetry;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using OpenTelemetry.Trace;

using IMessage = Messaging.Abstraction.IMessage;

namespace Messaging.RabbitMq.Consumer;

internal interface IMessageProcessor
{
    Task<AckStrategy> Process(ReadOnlyMemory<byte> body, MessageProperties properties,
        MessageReceivedInfo info, CancellationToken ct);
}
internal sealed class RabbitMqMessageProcessor<T> : IMessageProcessor where T : IMessage
{
    private static readonly Type _type = typeof(T);
    private readonly IServiceProvider _serviceProvider;

    public RabbitMqMessageProcessor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<AckStrategy> Process(ReadOnlyMemory<byte> body, MessageProperties properties,
        MessageReceivedInfo info, CancellationToken ct)
    {
        using var activity = RabbitMqTelemetry.RabbitMqActivitySource.Start("rabbitmq.consume", ActivityKind.Consumer, RabbitMqTelemetry.GetHeaderFromProps(properties).ActivityContext);
            await using var serviceScope = _serviceProvider.CreateAsyncScope(); 
            var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<MultiMessageRabbitMqMessageConsumer>>();
            
            try
            {
                if (activity is not null)
                {
                    activity.SetTag("messaging.rabbitmq.routing_key", info.RoutingKey);
                    activity.SetTag("messaging.exchange", info.Exchange);
                    activity.SetTag("messaging.destination", info.Queue);
                    activity.SetTag("messaging.timestamp", properties.Timestamp);
                    activity.SetTag("messaging.message_id", properties.MessageId);
                    activity.SetTag("messaging.message_type", properties.Type);
                    activity.SetTag("messaging.system", "rabbitmq");
                    activity.SetTag("messaging.destination_kind", "queue");
                    activity.SetTag("messaging.protocol", "AMQP");
                    activity.SetTag("messaging.protocol_version", "0.9.1");
                    activity.SetTag("messaging.message_name", _type.Name);
                }
                
                
                var serializer = serviceScope.ServiceProvider.GetRequiredService<ISerializer>();
                if (serializer.BytesToMessage(_type, body) is not T msg)
                {
                    logger.LogCantDeserializeMessage(info.Exchange, info.RoutingKey, info.Queue);
                    activity?.AddEvent(new ActivityEvent("Message is null or can't be deserialized"));
                    return AckStrategies.NackWithRequeue;
                }

                var subscriber = serviceScope.ServiceProvider.GetRequiredService<IMessageSubscriber<T>>();

                var result = await subscriber.Handle(msg, ct);
                if (!result.IsSuccess)
                {
                    logger.LogCantProcessMessage(result.ErrorValue, info.Exchange, info.RoutingKey, info.Queue, properties, info);
                    activity?.RecordException(result.ErrorValue);
                    return AckStrategies.NackWithRequeue;
                }
                
                return AckStrategies.Ack;
            }
            catch (Exception exc)
            {
                logger.LogCantProcessMessage(exc, info.Exchange, info.RoutingKey, info.Queue, properties, info);
                activity?.RecordException(exc);
                return AckStrategies.NackWithRequeue;
            }
    }
}

public sealed class MultiMessageRabbitMqMessageConsumer : BackgroundService
{
    private readonly IAdvancedBus _advancedBus;
    private readonly IServiceProvider _serviceProvider;
    private readonly MultiMessageRabbitMqSubscriptionConfiguration _subscriptionConfiguration;
    private IDisposable? _disposable;
    private ConcurrentDictionary<Type, Type> _processorTypes = new();
    
    public MultiMessageRabbitMqMessageConsumer(IAdvancedBus advancedBus, IServiceProvider serviceProvider, MultiMessageRabbitMqSubscriptionConfiguration subscriptionConfiguration)
    {
        _advancedBus = advancedBus;
        _serviceProvider = serviceProvider;
        _subscriptionConfiguration = subscriptionConfiguration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var exchange = await _advancedBus.ExchangeDeclareAsync(_subscriptionConfiguration.Exchange, configuration =>
        {
            configuration.AsDurable(true);
            configuration.WithType(ExchangeType.Topic);
        }, stoppingToken);

        var queue = await _advancedBus.QueueDeclareAsync(_subscriptionConfiguration.Queue, configuration =>
        {
            configuration.AsDurable(true);
            configuration.WithMessageTtl(TimeSpan.FromHours(5));
            configuration.WithMaxLength(10000);
        }, stoppingToken);

        foreach (var topic in _subscriptionConfiguration.Subscriptions.Keys)
        {
            _ = await _advancedBus.BindAsync(exchange, queue, topic, cancellationToken: stoppingToken);
        }
        

        _disposable = _advancedBus.Consume(queue, async (body, properties, info, ct) =>
        {
            await using var serviceScope = _serviceProvider.CreateAsyncScope(); 
            
            try
            {
                if (!_subscriptionConfiguration.Subscriptions.TryGetValue(info.RoutingKey, out var type))
                {
                    var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<MultiMessageRabbitMqMessageConsumer>>();
                    logger.LogSubscriberNotFound(info.Exchange, info.RoutingKey, info.Queue);
                    return AckStrategies.NackWithRequeue;
                }
                
                var processorType = _processorTypes.GetOrAdd(type.Message, t =>
                {
                    var processorType = typeof(RabbitMqMessageProcessor<>).MakeGenericType(t);
                    return processorType;
                });
                
                var processor = serviceScope.ServiceProvider.GetRequiredService(processorType);

                if (processor is not IMessageProcessor messageProcessor)
                {
                    var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<MultiMessageRabbitMqMessageConsumer>>();
                    logger.LogMessageProcessorNotFound(info.Exchange, info.RoutingKey, info.Queue);
                    return AckStrategies.NackWithoutRequeue;
                }
                
                return await messageProcessor.Process(body, properties, info, ct);
                
            }
            catch (Exception exc)
            {
                var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<MultiMessageRabbitMqMessageConsumer>>();
                logger.LogCantProcessMessage(exc, info.Exchange, info.RoutingKey, info.Queue, properties, info);
                return _subscriptionConfiguration.AckStrategy;
            }
        }, configuration =>
        {
            configuration.WithPrefetchCount(10);
        });

    }
    

    public override void Dispose()
    {
        _disposable?.Dispose();
        base.Dispose();
    }
}