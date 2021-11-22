using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShoppingList.Core.Repositories;
using ShoppingList.Infrastructure.Repositories;
using Refit;
using ShoppingList.Infrastructure.Refit;

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
            return services;
        }
    }
}
