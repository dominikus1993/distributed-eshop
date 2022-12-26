using System.Diagnostics;

using EasyNetQ;
using EasyNetQ.Consumer;
using EasyNetQ.Topology;

using Messaging.Abstraction;
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
}

public sealed class RabbitMqMessageConsumer<T> : BackgroundService where T : IMessage
{
    private readonly IAdvancedBus _advancedBus;
    private IServiceProvider _serviceProvider;
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
            configuration.WithExpires(TimeSpan.FromDays(5));
            configuration.WithMessageTtl(TimeSpan.FromHours(5));
            configuration.WithMaxLength(1000);
        }, stoppingToken);

        _ = await _advancedBus.BindAsync(exchange, queue, _subscriptionConfiguration.Topic, cancellationToken: stoppingToken);

        _disposable = _advancedBus.Consume(queue, async (body, properties, info, ct) =>
        {
            using var activity = RabbitMqTelemetry.RabbitMqActivitySource.Start("rabbitmq.consume", ActivityKind.Consumer, RabbitMqTelemetry.GetHeaderFromProps(properties).ActivityContext);
            await using var serviceScope = _serviceProvider.CreateAsyncScope(); 
            try
            {
                if (activity is not null)
                {
                    activity.SetTag("messaging.rabbitmq.routing_key", info.RoutingKey);
                    activity.SetTag("messaging.exchange", info.Exchange);
                    activity.SetTag("messaging.destination", info.Queue);
                    activity.SetTag("messaging.system", "rabbitmq");
                    activity.SetTag("messaging.destination_kind", "queue");
                    activity.SetTag("messaging.protocol", "AMQP");
                    activity.SetTag("messaging.protocol_version", "0.9.1");
                    activity.SetTag("messaging.message_name", typeof(T).FullName);
                }
                var serializer = serviceScope.ServiceProvider.GetRequiredService<ISerializer>();
                if (serializer.BytesToMessage(typeof(T), body) is not T message)
                {
                    var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<RabbitMqMessageConsumer<T>>>();
                    logger.LogWarning("Can't deserialize message {Exchange} -> {RoutingKey} -> {Queue}", info.Exchange, info.RoutingKey, info.Queue);
                    activity?.AddEvent(new ActivityEvent("Message is null or can't be deserialized"));
                    return AckStrategies.NackWithoutRequeue;
                }
                var subscriber = serviceScope.ServiceProvider.GetRequiredService<IMessageSubscriber<T>>();
                await subscriber.Handle(message, ct);
                return AckStrategies.Ack;
            }
            catch (Exception exc)
            {
                var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<RabbitMqMessageConsumer<T>>>();
                logger.LogWarning(exc, "Can't process message {Exchange} -> {RoutingKey} -> {Queue}", info.Exchange, info.RoutingKey, info.Queue);
                activity?.RecordException(exc);
                return AckStrategies.NackWithRequeue;
            }
        }, configuration =>
        {
            configuration.WithPrefetchCount(10);
        });

    }
    
    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            _disposable?.Dispose();
        }
    }

    public sealed override void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}