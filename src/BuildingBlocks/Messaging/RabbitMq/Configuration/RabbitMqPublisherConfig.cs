namespace Messaging.RabbitMq.Configuration;

public sealed class RabbitMqPublisherConfig<T>
{
    public required string Exchange { get; init; }
    public required string Topic { get; init; }
}