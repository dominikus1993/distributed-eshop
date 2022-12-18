using System.Text.Json;

using EasyNetQ;

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

public static class RabbitMqBuilderExtensions 
{
    
    public static RabbitMqBuilder AddRabbitMq(this WebApplicationBuilder builder)
    {
        var cfg = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqConfiguration>();
        if (cfg is null)
        {
            throw new InvalidOperationException("no rabbitmq configuration");
        }

        builder.Services.RegisterEasyNetQ(cfg.Connection, register => register.EnableSystemTextJson(new JsonSerializerOptions()));

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