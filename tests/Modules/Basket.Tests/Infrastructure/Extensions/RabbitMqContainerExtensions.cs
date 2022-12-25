using DotNet.Testcontainers.Containers;

namespace Basket.Tests.Infrastructure.Extensions;

public static class RabbitMqContainerExtensions
{
    public static string ConnectionString(this TestcontainerMessageBroker broker)
    {
        return $"amqp://{broker.Username}:{broker.Password}@{broker.Hostname}:{broker.Port}";
    }
}