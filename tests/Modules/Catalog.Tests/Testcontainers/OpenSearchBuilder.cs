using System.Net;
using System.Text;

using Docker.DotNet.Models;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

namespace Catalog.Tests.Testcontainers;

public sealed class OpenSearchBuilder : ContainerBuilder<OpenSearchBuilder, OpenSearchContainer, OpenSearchConfiguration>
{
    public const string ElasticsearchVmOptionsDirectoryPath = "/usr/share/elasticsearch/config/jvm.options.d/";

    public const string ElasticsearchDefaultMemoryVmOptionFileName = "elasticsearch-default-memory-vm.options";

    public const string ElasticsearchDefaultMemoryVmOptionFilePath = ElasticsearchVmOptionsDirectoryPath + ElasticsearchDefaultMemoryVmOptionFileName;

    public const string ElasticsearchImage = "opensearchproject/opensearch:2.7.0";

    public const ushort DefaultOpenSearchHttpPort = 9200;

    public const ushort OpenSearchTcpPort = 9300;

    public const string DefaultUsername = "admin";

    public const string DefaultPassword = "admin";

    private static readonly byte[] DefaultMemoryVmOption = Encoding.Default.GetBytes(string.Join("\n", "-Xms2147483648", "-Xmx2147483648"));

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchBuilder" /> class.
    /// </summary>
    public OpenSearchBuilder()
        : this(new OpenSearchConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private OpenSearchBuilder(OpenSearchConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override OpenSearchConfiguration DockerResourceConfiguration { get; }
    

    /// <inheritdoc />
    public override OpenSearchContainer Build()
    {
        Validate();
        return new OpenSearchContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override OpenSearchBuilder Init()
    {
        var builder = base.Init()
            .WithImage(ElasticsearchImage)
            .WithPortBinding(DefaultOpenSearchHttpPort, true)
            .WithPortBinding(OpenSearchTcpPort, true)
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword)
            .WithEnvironment("discovery.type", "single-node")
            .WithResourceMapping(DefaultMemoryVmOption, ElasticsearchDefaultMemoryVmOptionFilePath);
            
            return builder.WithWaitStrategy(Wait.ForUnixContainer()
                .UntilPortIsAvailable(DefaultOpenSearchHttpPort)
                .AddCustomWaitStrategy(new WaitUntil(builder.DockerResourceConfiguration)));
    }
    
    /// <summary>
    /// Sets the Elasticsearch username.
    /// </summary>
    /// <remarks>
    /// The Docker image does not allow to configure the username.
    /// </remarks>
    /// <param name="username">The OpenSearch username.</param>
    /// <returns>A configured instance of <see cref="OpenSearchBuilder" />.</returns>
    private OpenSearchBuilder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new OpenSearchConfiguration(username: username));
    }
    
    
    /// <summary>
    /// Sets the OpenSearch password.
    /// </summary>
    /// <remarks>
    /// The Docker image does not allow to configure the username.
    /// </remarks>
    /// <param name="password">The OpenSearch username.</param>
    /// <returns>A configured instance of <see cref="OpenSearchBuilder" />.</returns>
    private OpenSearchBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new OpenSearchConfiguration(password: password));
    }
    
    /// <inheritdoc />
    protected override OpenSearchBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new OpenSearchConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override OpenSearchBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new OpenSearchConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override OpenSearchBuilder Merge(OpenSearchConfiguration oldValue, OpenSearchConfiguration newValue)
    {
        return new OpenSearchBuilder(new OpenSearchConfiguration(oldValue, newValue));
    }
    
    /// <inheritdoc cref="IWaitUntil" />
    private sealed class WaitUntil : IWaitUntil
    {
        private readonly OpenSearchConfiguration _searchConfiguration;

        public WaitUntil(OpenSearchConfiguration searchConfiguration)
        {
            _searchConfiguration = searchConfiguration;
        }


        /// <inheritdoc />
        public async Task<bool> UntilAsync(IContainer container)
        {
            using var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            using var client = new HttpClient(handler);
            var port = container.GetMappedPublicPort(DefaultOpenSearchHttpPort);
            var uri = new UriBuilder(Uri.UriSchemeHttps, container.Hostname, port)
            {
                Path = "/"
            };
           
            using var request = new HttpRequestMessage(HttpMethod.Get, uri.Uri);
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(string.Join(":", _searchConfiguration.Username, _searchConfiguration.Password))));
            var result = await client.SendAsync(request);

            return result.StatusCode is HttpStatusCode.OK or HttpStatusCode.Unauthorized;
        }
    }
}