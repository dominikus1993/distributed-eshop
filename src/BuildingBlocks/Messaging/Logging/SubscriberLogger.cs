using Microsoft.Extensions.Logging;

namespace Messaging.Logging;

internal static partial class SubscriberLogger
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Error,
        Message = "Can't process message {Exchange} -> {RoutingKey} -> {Queue}")]
    public static partial void LogCantProcessMessage(
        this ILogger logger, Exception exception, string exchange, string routingKey, string queue);
    
    
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Warning,
        Message = "Can't deserialize message {Exchange} -> {RoutingKey} -> {Queue}")]
    public static partial void LogCantDeserializeMessage(
        this ILogger logger, string exchange, string routingKey, string queue);
}