namespace Messaging.Nats.Configuration;

public sealed class NatsConfiguration
{
    
}

public sealed class NatsPublisherConfig<T>
{
    public string Subject { get; set; } = "eshop";
}