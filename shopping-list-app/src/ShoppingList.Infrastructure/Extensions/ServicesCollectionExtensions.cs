using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShoppingList.Core.Repositories;
using ShoppingList.Infrastructure.Repositories;
using Grpc.Core;
using Grpc.Net.Client;

namespace ShoppingList.Infrastructure.Extensions
{
    public static class ServicesCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton(sp => {
                var channel = GrpcChannel.ForAddress(config.GetConnectionString("Storage"));
                var client = new ShoppingListsStorage.ShoppingListsStorage.ShoppingListsStorageClient(channel);
                return client;
            });
            services.AddTransient<IShoppingListRepository, GrpcShoppingListRepository>();
            return services;
        }
    }
}
