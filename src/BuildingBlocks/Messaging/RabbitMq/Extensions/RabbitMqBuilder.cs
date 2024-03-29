using System.Text.Json;

using EasyNetQ;
using EasyNetQ.ConnectionString;

using Messaging.Abstraction;
using Messaging.Configuration;
using Messaging.RabbitMq.Configuration;
using Messaging.RabbitMq.Publisher;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using IMessage = Messaging.Abstraction.IMessage;

namespace Messaging.RabbitMq.Extensions;

public sealed class RabbitMqBuilder
{
    public required IServiceCollection Services { get; init; }
    public required IConfiguration Configuration { get; init; }

    internal RabbitMqBuilder()
    {
        
    }
}

public sealed class RabbitMqConfiguration
{
    internal JsonSerializerOptions JsonSerializerOptions { get; private set; } = JsonSerializerOptions.Default;
    
    
    public RabbitMqConfiguration SetJsonSerializerOptions(JsonSerializerOptions jsonSerializerOptions)
    {
        ArgumentNullException.ThrowIfNull(jsonSerializerOptions);
        JsonSerializerOptions = jsonSerializerOptions;
        return this;
    }
}

public static class RabbitMqBuilderExtensions 
{
    public static RabbitMqBuilder AddRabbitMq(this IServiceCollection services, IConfiguration configuration, Action<RabbitMqConfiguration>? configAction = null)
    {
        var cfg = configuration.GetRequiredSection("RabbitMq").Get<RabbitMqConnectionConfiguration>();
        if (cfg is null)
        {
            throw new InvalidOperationException("no rabbitmq configuration");
        }

        var config = new RabbitMqConfiguration(); 
        configAction?.Invoke(config);
        
        services.RegisterEasyNetQ(_ =>
        {
            var parser = new AmqpConnectionStringParser().Parse(cfg.Connection);
            return parser;
        }, register =>
        {
            register.EnableSystemTextJson(config.JsonSerializerOptions);
        });

        return new RabbitMqBuilder() { Services = services, Configuration = configuration };
    }

    private static RabbitMqBuilder AddPublisher<T>(this RabbitMqBuilder builder, Action<RabbitMqPublisherConfig<T>> action) where T : IMessage
    {
        var cfg = new RabbitMqPublisherConfig<T>();
        action.Invoke(cfg);
        builder.Services.AddSingleton(cfg);
        builder.Services.AddTransient<IMessagePublisher<T>, RabbitMqMessagePublisher<T>>();
        return builder;
    }
    
    public static RabbitMqBuilder AddPublisher<T>(this RabbitMqBuilder builder, string exchange = "eshop", string topic = "#") where T : IMessage
    {
        return AddPublisher<T>(builder, config =>
        {
            config.Exchange = exchange;
            config.Topic = topic;
        });
    }
}