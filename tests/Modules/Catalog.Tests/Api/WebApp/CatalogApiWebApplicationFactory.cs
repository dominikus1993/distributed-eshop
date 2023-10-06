using Catalog.Infrastructure.Extensions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Catalog.Tests.Api.WebApp;

public sealed class CatalogApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly OpenSearchConfiguration _searchConfiguration;

    public CatalogApiWebApplicationFactory(OpenSearchConfiguration searchConfiguration)
    {
        _searchConfiguration = searchConfiguration;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("OpenSearch:Url", _searchConfiguration.Url.ToString());
        builder.UseSetting("OpenSearch:UserName", _searchConfiguration.UserName);
        builder.UseSetting("OpenSearch:Password", _searchConfiguration.Password);
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("./Api/appsettings.json", optional: false, reloadOnChange: true);
        });
        base.ConfigureWebHost(builder);
    }
}