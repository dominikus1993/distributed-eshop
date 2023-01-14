using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Core.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static IServiceCollection AddCatalog(this IServiceCollection services)
    {
        services.AddMediator();
        return services;
    }
}