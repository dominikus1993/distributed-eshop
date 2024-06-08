using EasyNetQ.Consumer;

using Messaging.Abstraction;
using Messaging.RabbitMq.Configuration;

namespace Messaging.RabbitMq.Consumer;

public sealed class MultiMessageRabbitMqSubscription
{
    public string RouteKey { get; set; } = "#";

    private bool Equals(MultiMessageRabbitMqSubscription other)
    {
        return RouteKey == other.RouteKey;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is MultiMessageRabbitMqSubscription other && Equals(other);
    }

    public override int GetHashCode()
    {
        return RouteKey.GetHashCode();
    }
}

public sealed class MultiMessageRabbitMqSubscriptionConfiguration
{
    public string? Exchange { get; set; }
    public string? Queue  {get; set; }
    
    internal Dictionary<string, (Type Message, Type Handler)> Subscriptions { get; set; } = new();
    
    public MultiMessageRabbitMqSubscriptionConfiguration AddSubscription<T, THandler>(MultiMessageRabbitMqSubscription subscription) where T : IMessage where THandler : IMessageSubscriber<T>
    {
        ArgumentException.ThrowIfNullOrEmpty(subscription.RouteKey);
        Subscriptions.Add(subscription.RouteKey, (Message: typeof(T), Handler: typeof(THandler)));
        return this;
    }
    
    public DefaultErrorHandlingStrategy DefaultErrorHandlingStrategy { get; set; } =
        DefaultErrorHandlingStrategy.NackWithRequeue;
    internal AckStrategy AckStrategy => DefaultErrorHandlingStrategy.Convert();
}