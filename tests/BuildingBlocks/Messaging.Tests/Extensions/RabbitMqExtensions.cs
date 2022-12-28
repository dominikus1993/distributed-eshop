using System.Runtime.CompilerServices;

using EasyNetQ;
using EasyNetQ.Topology;

namespace Messaging.Tests.Extensions;

public static class RabbitMqExtensions
{
    public static async Task<PullResult<T>> ConsumeOne<T>(this IAdvancedBus bus, Queue queue, CancellationToken cancellationToken = default)
    {
        using var consumer = bus.CreatePullingConsumer<T>(queue);

        return await consumer.PullAsync(cancellationToken);
    }
    
    public static async IAsyncEnumerable<PullResult<T>> ConsumeEnumerable<T>(this IAdvancedBus bus, Queue queue, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var consumer = bus.CreatePullingConsumer<T>(queue);

        while (!cancellationToken.IsCancellationRequested)
        {
            var result = await consumer.PullAsync(cancellationToken);

            yield return result;
        }
        
    }
}