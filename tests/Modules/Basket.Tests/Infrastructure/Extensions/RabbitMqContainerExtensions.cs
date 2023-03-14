using DotNet.Testcontainers.Containers;

using Testcontainers.RabbitMq;

namespace Basket.Tests.Infrastructure.Extensions;

public static class RabbitMqContainerExtensions
{
    public static string ConnectionString(this RabbitMqContainer broker)
    {
        return broker.GetConnectionString();
    }
}