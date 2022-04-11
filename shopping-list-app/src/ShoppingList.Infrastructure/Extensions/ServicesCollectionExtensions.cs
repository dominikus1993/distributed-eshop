using System.Collections.Immutable;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShoppingList.Core.Repositories;
using ShoppingList.Infrastructure.Repositories;
using Refit;
using ShoppingList.Infrastructure.Refit;
using ShoppingList.Infrastructure.OpenTelemetry;
using ShoppingList.Core.UseCases;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ShoppingList.Infrastructure.Extensions
{
    public static class ServicesCollectionExtensions
    {
        public static IServiceCollection AddApiInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services
                .AddRefitClient<IStorageClient>()
                .ConfigureHttpClient(client =>
                {
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.BaseAddress = new Uri(config.GetConnectionString("Storage"));
                });
            services.AddTransient<IShoppingListRepository, HttpShoppingListRepository>();
            return services;
        }

        public static IServiceCollection AddGrpcInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddGrpcClient<ShoppingListsStorage.ShoppingListsStorage.ShoppingListsStorageClient>(options =>
            {
                options.Address = new Uri(config.GetConnectionString("Storage"));
            });
            services.AddTransient<IShoppingListRepository, GrpcShoppingListRepository>();
            services.Decorate<GetCustomerShoppingListUseCase, TracedGetCustomerShoppingListUseCase>();
            return services;
        }

        public static IHealthChecksBuilder AddApiHealthCheck(this IHealthChecksBuilder builder, IConfiguration config)
        {
            var sUrl = new Uri(config.GetConnectionString("Storage"));
            var pingUrl = new Uri(sUrl, "ping");
            builder.AddUrlGroup(
                pingUrl,
                name: "Storage",
                failureStatus: HealthStatus.Unhealthy,
                tags: new string[] { "storage" });
            return builder;
        }
    }
}
