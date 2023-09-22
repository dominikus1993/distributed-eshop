using System.Runtime.CompilerServices;

using EasyNetQ.Consumer;

namespace Messaging.RabbitMq.Configuration;

internal static class RabbitMqPublisherConfigExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AckStrategy Convert(this DefaultErrorHandlingStrategy strategy) => strategy switch
    {
        DefaultErrorHandlingStrategy.NackWithoutRequeue => AckStrategies.NackWithoutRequeue,
        DefaultErrorHandlingStrategy.NackWithRequeue => AckStrategies.NackWithRequeue,
        _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
    };
}