using System.Diagnostics;

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

public sealed class RabbitMqSubscriptionConfiguration<T> where T : IMessage
{
    public string Topic { get; set; } = "#";
    public string? Exchange { get; set; }
    public string? Queue  {get; set; }
    public string MessageName => typeof(T).FullName!;
    public DefaultErrorHandlingStrategy DefaultErrorHandlingStrategy { get; set; } =
        DefaultErrorHandlingStrategy.NackWithRequeue;
    internal AckStrategy AckStrategy => DefaultErrorHandlingStrategy.Convert();
}

public sealed class RabbitMqMessageConsumer<T> : BackgroundService where T : IMessage
{
    private static readonly Type _type = typeof(T);
    private readonly IAdvancedBus _advancedBus;
    private readonly IServiceProvider _serviceProvider;
    private readonly RabbitMqSubscriptionConfiguration<T> _subscriptionConfiguration;
    private IDisposable? _disposable;

    public RabbitMqMessageConsumer(IAdvancedBus advancedBus, IServiceProvider serviceProvider, RabbitMqSubscriptionConfiguration<T> subscriptionConfiguration)
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

        _ = await _advancedBus.BindAsync(exchange, queue, _subscriptionConfiguration.Topic, cancellationToken: stoppingToken);

        _disposable = _advancedBus.Consume(queue, async (body, properties, info, ct) =>
        {
            using var activity = RabbitMqTelemetry.RabbitMqActivitySource.Start("rabbitmq.consume", ActivityKind.Consumer, RabbitMqTelemetry.GetHeaderFromProps(properties).ActivityContext);
            await using var serviceScope = _serviceProvider.CreateAsyncScope(); 
            var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<RabbitMqMessageConsumer<T>>>();
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
                    activity.SetTag("messaging.message_name", _subscriptionConfiguration.MessageName);
                }
                var serializer = serviceScope.ServiceProvider.GetRequiredService<ISerializer>();
                if (serializer.BytesToMessage(_type, body) is not T message)
                {
                    logger.LogCantDeserializeMessage(info.Exchange, info.RoutingKey, info.Queue);
                    activity?.AddEvent(new ActivityEvent("Message is null or can't be deserialized"));
                    return AckStrategies.NackWithRequeue;
                }
                var subscriber = serviceScope.ServiceProvider.GetRequiredService<IMessageSubscriber<T>>();
                var result = await subscriber.Handle(message, ct);
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