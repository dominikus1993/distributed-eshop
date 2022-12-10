namespace Messaging.RabbitMq.Configuration;

public sealed class RabbitMqPublisherConfig<T>
{
    public string Exchange { get; set; } = "eshop";
    public string Topic { get; set; } = "#";
}