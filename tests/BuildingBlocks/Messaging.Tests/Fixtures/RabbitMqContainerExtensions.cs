using DotNet.Testcontainers.Containers;

namespace Messaging.Tests.Fixtures;

public static class RabbitMqContainerExtensions
{
    public static string ConnectionString(this TestcontainerMessageBroker broker)
    {
        return $"amqp://{broker.Username}:{broker.Password}@{broker.Hostname}:{broker.Port}";
    }
}