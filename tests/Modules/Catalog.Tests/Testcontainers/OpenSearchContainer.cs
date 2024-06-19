using DotNet.Testcontainers.Containers;

using Microsoft.Extensions.Logging;

namespace Catalog.Tests.Testcontainers;

public sealed class OpenSearchContainer : DockerContainer
{
    private readonly OpenSearchConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public OpenSearchContainer(OpenSearchConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the Elasticsearch connection string.
    /// </summary>
    /// <remarks>
    /// The Elasticsearch module does not export the SSL certificate from the container by default.
    /// If you are trying to connect to the Elasticsearch service, you need to override the certificate validation callback to establish the connection.
    /// We will export the certificate and support trusted SSL connections in the future.
    /// </remarks>
    /// <returns>The Elasticsearch connection string.</returns>
    public Uri GetConnectionString()
    {
        var endpoint = new UriBuilder(Uri.UriSchemeHttps, Hostname, GetMappedPublicPort(OpenSearchBuilder.DefaultOpenSearchHttpPort))
        {
            UserName = _configuration.Username,
            Password = _configuration.Password
        };
        return endpoint.Uri;
    }
    
    public Catalog.Infrastructure.Extensions.OpenSearchConfiguration GetConfiguration()
    {
        var endpoint = new UriBuilder(Uri.UriSchemeHttps, Hostname,
            GetMappedPublicPort(OpenSearchBuilder.DefaultOpenSearchHttpPort));
        return new Catalog.Infrastructure.Extensions.OpenSearchConfiguration()
        {
            Url = endpoint.Uri, UserName = _configuration.Username, Password = _configuration.Password,
        };
    }
}