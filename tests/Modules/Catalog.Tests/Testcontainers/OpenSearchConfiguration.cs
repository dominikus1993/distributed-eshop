using Docker.DotNet.Models;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;

namespace Catalog.Tests.Testcontainers;

public sealed class OpenSearchConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchConfiguration" /> class.
    /// </summary>
    /// <param name="username">The Elasticsearch username.</param>
    /// <param name="password">The Elasticsearch password.</param>
    public OpenSearchConfiguration(
        string? username = null,
        string? password = null)
    {
        Username = username!;
        Password = password!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public OpenSearchConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public OpenSearchConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public OpenSearchConfiguration(OpenSearchConfiguration resourceConfiguration)
        : this(new OpenSearchConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public OpenSearchConfiguration(OpenSearchConfiguration oldValue, OpenSearchConfiguration newValue)
        : base(oldValue, newValue)
    {
        Username = BuildConfiguration.Combine(oldValue.Username, newValue.Username);
        Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
    }

    /// <summary>
    /// Gets the Elasticsearch username.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Gets the Elasticsearch password.
    /// </summary>
    public string Password { get; }
}