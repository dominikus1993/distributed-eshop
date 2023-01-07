using System.Diagnostics;
using System.Text;

using EasyNetQ;

using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Trace;

namespace Messaging.RabbitMq.Telemetry;

public static class RabbitMqTelemetry
{
    internal const string RabbitMqActivitySourceName = $"{nameof(Messaging)}.{nameof(RabbitMq)}";

    internal static readonly ActivitySource RabbitMqActivitySource = new(RabbitMqActivitySourceName, "v1.0.0");

    public static Activity? Start(this ActivitySource source, string name = "rabbitmq.publish",
        ActivityKind kind = ActivityKind.Producer, ActivityContext? activityContext = null)
    {
        if (activityContext.HasValue)
        {
            return source.StartActivity(name, kind, parentContext: activityContext.Value);
        }
        return source.StartActivity(name, kind);
    }
    
    public static TracerProviderBuilder AddRabbitMqTelemetry(this TracerProviderBuilder builder)
    {
        return builder.AddSource(RabbitMqActivitySourceName);
    }
    
    internal static void AddActivityToHeader(Activity activity, MessageProperties props)
    {
        var context = new PropagationContext(activity.Context, Baggage.Current);
        Propagators.DefaultTextMapPropagator.Inject(context, props,(properties, key, value) =>  InjectContextIntoHeader(properties, key, value));
    }
    
    internal static PropagationContext GetHeaderFromProps(MessageProperties props)
    {
        var context = new PropagationContext();
        return Propagators.DefaultTextMapPropagator.Extract(context, props, (properties, s) => ExtractContextFromHeader(properties, s));
    }

    private static void InjectContextIntoHeader(MessageProperties props, string key, string value)
    {
        props.Headers ??= new Dictionary<string, object>();
        props.Headers.Add(key, value);
    }
    
    private static string[] ExtractContextFromHeader(MessageProperties props, string key)
    {
        if (props.Headers is null)
        {
            return Array.Empty<string>();
        }

        if (!props.Headers.TryGetValue(key, out var value) || value is not byte[] result)
        {
            return Array.Empty<string>();
        }

        var res = Encoding.UTF8.GetString(result);
        return new[] { res };
    }
}