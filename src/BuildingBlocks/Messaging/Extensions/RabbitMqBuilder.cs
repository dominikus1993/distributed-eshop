using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using EasyNetQ;
using EasyNetQ.ConnectionString;
using EasyNetQ.DI;

using Messaging.Abstraction;
using Messaging.Configuration;
using Messaging.RabbitMq.Configuration;
using Messaging.RabbitMq.Publisher;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using IMessage = Messaging.Abstraction.IMessage;

namespace Messaging.Extensions;

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
    public IJsonTypeInfoResolver? JsonTypeInfoResolver { get; private set; }

    public RabbitMqConfiguration SetJsonTypeInfoResolver(IJsonTypeInfoResolver jsonTypeInfoResolver)
    {
        JsonTypeInfoResolver = jsonTypeInfoResolver;
        return this;
    }
}

public static class RabbitMqBuilderExtensions 
{
    public static RabbitMqBuilder AddRabbitMq(this WebApplicationBuilder builder, Action<RabbitMqConfiguration>? configAction = null)
    {
        var cfg = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqConnectionConfiguration>();
        if (cfg is null)
        {
            throw new InvalidOperationException("no rabbitmq configuration");
        }

        var config = new RabbitMqConfiguration(); 
        configAction?.Invoke(config);
        
        builder.Services.RegisterEasyNetQ(resolve =>
        {
            var parser = new AmqpConnectionStringParser().Parse(cfg.Connection);
            return parser;
        }, register =>
        {
            register.EnableSystemTextJson(
                                     new JsonSerializerOptions() { TypeInfoResolver = config.JsonTypeInfoResolver });
        });

        return new RabbitMqBuilder() { Services = builder.Services, Configuration = builder.Configuration };
    }

    private static RabbitMqBuilder AddPublisher<T>(this RabbitMqBuilder builder, Action<RabbitMqPublisherConfig<T>> action) where T : IMessage
    {
        var cfg = new RabbitMqPublisherConfig<T>();
        action?.Invoke(cfg);
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