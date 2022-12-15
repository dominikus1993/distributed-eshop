using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Basket.Core.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static IServiceCollection AddBasket(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediator();
        return services;
    }
}