namespace Messaging.RabbitMq.Configuration;

public enum DefaultErrorHandlingStrategy
{
    NackWithRequeue = 0,
    NackWithoutRequeue = 1,
}

public sealed class RabbitMqPublisherConfig<T>
{
    public string Exchange { get; set; } = "eshop";
    public string Topic { get; set; } = "#";

    public TimeSpan? TTL { get; set; } = null;
}