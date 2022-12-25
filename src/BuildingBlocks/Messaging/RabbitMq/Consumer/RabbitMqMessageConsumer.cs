using EasyNetQ;
using EasyNetQ.Consumer;
using EasyNetQ.Topology;

using Microsoft.Extensions.Hosting;
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
    private IAdvancedBus _advancedBus;
    private IServiceProvider _serviceProvider;
    private RabbitMqSubscriptionConfiguration<T> _subscriptionConfiguration;
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

        var binding = await _advancedBus.BindAsync(exchange, queue, _subscriptionConfiguration.Topic, cancellationToken: stoppingToken);

        _advancedBus.Consume(queue, async (body, properties, info, ct) =>
        {
            
            return AckStrategies.Ack;
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