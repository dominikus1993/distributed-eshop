using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Basket.Core.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddBasket(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediator();
        return builder;
    }
}