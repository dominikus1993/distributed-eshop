using System.Reflection;

namespace Telemetry.OpenTelemetry;

public readonly record struct Tag(string Name, string Value);

public sealed class OpenTelemetryConfiguration
{
    private bool _openTelemetryEnabled;
    private bool _openTelemetryLoggingEnabled;
    private bool _openTelemetryMetricsEnabled;
    private bool _oltpExporterEnabled;
    private bool _consoleExporterEnabled;

    public bool OpenTelemetryEnabled
    {
        get =>
            bool.TryParse(Environment.GetEnvironmentVariable("OPENTELEMETRY_ENABLED"),
                out var openTelemetryEnabled) ? openTelemetryEnabled : _openTelemetryEnabled;
        set => _openTelemetryEnabled = value;
    }

    public bool OpenTelemetryLoggingEnabled
    {
        get => bool.TryParse(Environment.GetEnvironmentVariable("OPENTELEMETRY_LOGGING_ENABLED"), out var otle) ? otle : _openTelemetryLoggingEnabled;
        set => _openTelemetryLoggingEnabled = value;
    }

    public bool OpenTelemetryMetricsEnabled
    {
        get => bool.TryParse(Environment.GetEnvironmentVariable("OPENTELEMETRY_METRICS_ENABLED"), out var ome) ? ome : _openTelemetryMetricsEnabled;
        set => _openTelemetryMetricsEnabled = value;
    }

    public bool OltpExporterEnabled
    {
        get => bool.TryParse(Environment.GetEnvironmentVariable("OPENTELEMETRY_OLTP_EXPORTER_ENABLED"), out var ome) ? ome : _oltpExporterEnabled;
        set => _oltpExporterEnabled = value;
    }

    public bool ConsoleExporterEnabled
    {
        get => bool.TryParse(Environment.GetEnvironmentVariable("OPENTELEMETRY_CONSOLE_EXPORTER_ENABLED"), out var ome) ? ome : _consoleExporterEnabled;
        set => _consoleExporterEnabled = value;
    }
}

public sealed class Service
{
    public IReadOnlyCollection<Tag>? Tags { get; init; } = null!;
    public string Name { get; init; } = Assembly.GetExecutingAssembly().FullName!;
    public string Version { get; init; } = "Unspecified";
    
    public OpenTelemetryConfiguration OpenTelemetryConfiguration { get; init; } = new();
}