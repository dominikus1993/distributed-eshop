using Chat.Core.Repostories;
using Chat.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Infrastructure.Framework;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IMessagesRepository, FakeMessageRepository>();
        return services;
    }
}