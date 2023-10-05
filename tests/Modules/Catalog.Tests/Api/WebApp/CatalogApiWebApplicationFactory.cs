using Catalog.Infrastructure.Extensions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Catalog.Tests.Api.WebApp;

public class CatalogApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private OpenSearchConfiguration _searchConfiguration;

    public CatalogApiWebApplicationFactory(OpenSearchConfiguration searchConfiguration)
    {
        _searchConfiguration = searchConfiguration;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("./Api/appsettings.json", optional: false, reloadOnChange: true);
            var dict = new Dictionary<string, string>()
            {
                { "OpenSearch:Url", _searchConfiguration.Url.ToString() },
                { "OpenSearch:UserName", _searchConfiguration.UserName },
                { "OpenSearch:Password", _searchConfiguration.Password },
            };
            configurationBuilder.AddInMemoryCollection(dict!);
        });
        base.ConfigureWebHost(builder);
    }
}