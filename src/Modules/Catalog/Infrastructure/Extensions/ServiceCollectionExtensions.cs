using Catalog.Core.Repository;
using Catalog.Infrastructure.Repositories;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("OpenSearch").Get<OpenSearchConfiguration>()!;
        var client = OpenSearchInstaller.Setup(settings);
        services.AddSingleton(client);
        services.AddTransient<IProductFilter, OpenSearchProductFilter>();
        services.AddTransient<IProductReader, OpenSearchProductReader>();
        return services;
    }
}