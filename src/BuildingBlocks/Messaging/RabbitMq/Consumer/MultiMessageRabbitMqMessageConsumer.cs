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


public sealed class MultiMessageRabbitMqMessageConsumer : BackgroundService
{
    private readonly IAdvancedBus _advancedBus;
    private readonly IServiceProvider _serviceProvider;
    private readonly MultiMessageRabbitMqSubscriptionConfiguration _subscriptionConfiguration;
    private IDisposable? _disposable;
    private ConcurrentDictionary<Type, MethodInfo> _methodInfos = new();
    
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
                }

                if (!_subscriptionConfiguration.Subscriptions.TryGetValue(info.RoutingKey, out var type))
                {
                    logger.LogSubscriberNotFound(info.Exchange, info.RoutingKey, info.Queue);
                    return AckStrategies.NackWithRequeue;
                }
                
                activity?.SetTag("messaging.message_name", type.Message.Name);
                
                var serializer = serviceScope.ServiceProvider.GetRequiredService<ISerializer>();
                if (serializer.BytesToMessage(type.Message, body) is not {} msg)
                {
                    logger.LogCantDeserializeMessage(info.Exchange, info.RoutingKey, info.Queue);
                    activity?.AddEvent(new ActivityEvent("Message is null or can't be deserialized"));
                    return AckStrategies.NackWithRequeue;
                }

                var subscriber = serviceScope.ServiceProvider.GetRequiredService(type.Handler);
                var handle = _methodInfos.GetOrAdd(type.Message,
                    (messageType) =>
                        typeof(IMessageSubscriber<>).MakeGenericType(messageType).GetMethod(nameof(IMessageSubscriber<IMessage>.Handle))!);
                
                var result = await (Task<Result<Unit>>)handle.Invoke(subscriber, [msg, ct])!;
                if (!result.IsSuccess)
                {
                    logger.LogCantProcessMessage(result.ErrorValue, info.Exchange, info.RoutingKey, info.Queue, properties);
                    activity?.RecordException(result.ErrorValue);
                    return _subscriptionConfiguration.AckStrategy;
                }
                
                return AckStrategies.Ack;
            }
            catch (Exception exc)
            {
                logger.LogCantProcessMessage(exc, info.Exchange, info.RoutingKey, info.Queue, properties);
                activity?.RecordException(exc);
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