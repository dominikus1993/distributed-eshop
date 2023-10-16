using Catalog.Infrastructure.Extensions;

using Meziantou.Extensions.Logging.Xunit;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Xunit.Abstractions;

namespace Catalog.Tests.Api.WebApp;

public sealed class CatalogApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly OpenSearchConfiguration _searchConfiguration;
    private readonly ITestOutputHelper _testOutputHelper;

    public CatalogApiWebApplicationFactory(OpenSearchConfiguration searchConfiguration, ITestOutputHelper testOutputHelper)
    {
        _searchConfiguration = searchConfiguration;
        _testOutputHelper = testOutputHelper;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("OpenSearch:Url", _searchConfiguration.Url.ToString());
        builder.UseSetting("OpenSearch:UserName", _searchConfiguration.UserName);
        builder.UseSetting("OpenSearch:Password", _searchConfiguration.Password);
        builder.ConfigureLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.Services.AddSingleton<ILoggerProvider>(new XUnitLoggerProvider(_testOutputHelper));
        });
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("./Api/appsettings.json", optional: false, reloadOnChange: true);
        });
        base.ConfigureWebHost(builder);
    }
}