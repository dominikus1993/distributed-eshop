using DotNet.Testcontainers.Containers;

using Testcontainers.RabbitMq;

namespace Messaging.Tests.Fixtures;

public static class RabbitMqContainerExtensions
{
    public static string ConnectionString(this RabbitMqContainer broker)
    {
        return broker.GetConnectionString();
    }
}