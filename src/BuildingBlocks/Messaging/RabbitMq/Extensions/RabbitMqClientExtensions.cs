using EasyNetQ;
using EasyNetQ.Topology;

using Messaging.RabbitMq.Configuration;

namespace Messaging.RabbitMq.Extensions;

internal static class RabbitMqClientExtensions
{
    public static Task<Exchange> DeclareExchangeAsync<T>(this IAdvancedBus bus, RabbitMqPublisherConfig<T> cfg, CancellationToken cancellationToken = default)
    {
        return bus.ExchangeDeclareAsync(cfg.Exchange, configuration =>
        {
            configuration.AsDurable(true);
            configuration.WithType(ExchangeType.Topic);
        }, cancellationToken);
    }
}