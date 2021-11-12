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
using System.Net.Http;
using Grpc.Core.Interceptors;
using Datadog.Trace;

namespace ShoppingList.Infrastructure.Extensions
{
    public static class ServicesCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
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
