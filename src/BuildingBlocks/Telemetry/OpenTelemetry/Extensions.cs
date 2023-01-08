using Hosting.OpenTelemetry;

using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Telemetry.OpenTelemetry;

using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public sealed class SampleOpenTelemetryBuilder
{
    public SampleOpenTelemetryBuilder(OpenTelemetryBuilder openTelemetryBuilder, IHostEnvironment environment,
        Service service)
    {
        OpenTelemetryBuilder = openTelemetryBuilder;
        Environment = environment;
        Service = service;
    }

    public Service Service { get; }

    public OpenTelemetryBuilder OpenTelemetryBuilder
    {
        get;
    }

    public IHostEnvironment Environment
    {
        get;
    }
}

public static class OpenTelemetryExtensions
{
    public static SampleOpenTelemetryBuilder AddOpenTelemetry(this WebApplicationBuilder builder,
        Service? configure = null)
    {
        var config = configure ?? new Service();
        var otelBuilder = builder.Services.AddOpenTelemetry();
        return new SampleOpenTelemetryBuilder(otelBuilder, builder.Environment, config);
    }

    public static SampleOpenTelemetryBuilder AddOpenTelemetryTracing(this SampleOpenTelemetryBuilder builder,
        Action<TracerProviderBuilder>? setup = null)
    {
        if (builder.Service.OpenTelemetryConfiguration.OpenTelemetryEnabled)
        {
            builder.OpenTelemetryBuilder.WithTracing(b =>
            {
                b.AddAspNetCoreInstrumentation(options =>
                {
                    options.RecordException = true;
                });
                setup?.Invoke(b);
                b.SetResourceBuilder(builder.Service.GetResourceBuilder(builder.Environment.EnvironmentName));
                if (builder.Service.OpenTelemetryConfiguration.OltpExporterEnabled)
                {
                    b.AddOtlpExporter();
                }

                if (builder.Service.OpenTelemetryConfiguration.ConsoleExporterEnabled)
                {
                    b.AddConsoleExporter();
                }
            });
        }

        return builder;
    }

    public static WebApplicationBuilder AddOpenTelemetryLogging(this WebApplicationBuilder builder,
        Service? configure = null, Action<OpenTelemetryLoggerOptions>? setup = null)
    {
        var config = configure ?? new Service();
        if (config.OpenTelemetryConfiguration.OpenTelemetryLoggingEnabled)
        {
            builder.Logging.AddOpenTelemetry(b =>
            {
                b.IncludeFormattedMessage = true;
                b.IncludeScopes = true;
                b.ParseStateValues = true;
                b.AttachLogsToActivityEvent();
                b.SetResourceBuilder(config.GetResourceBuilder(builder.Environment.EnvironmentName));
                setup?.Invoke(b);
                if (config.OpenTelemetryConfiguration.OltpExporterEnabled)
                {
                    b.AddOtlpExporter();
                }

                if (config.OpenTelemetryConfiguration.ConsoleExporterEnabled)
                {
                    b.AddConsoleExporter();
                }
            });
        }

        return builder;
    }

    public static SampleOpenTelemetryBuilder AddOpenTelemetryMetrics(this SampleOpenTelemetryBuilder builder,
        Action<MeterProviderBuilder>? setup = null)
    {
        if (builder.Service.OpenTelemetryConfiguration!.OpenTelemetryMetricsEnabled)
        {
            builder.OpenTelemetryBuilder.WithMetrics(b =>
            {
                b.AddAspNetCoreInstrumentation();
                b.SetResourceBuilder(builder.Service.GetResourceBuilder(builder.Environment.EnvironmentName));
                setup?.Invoke(b);
                if (builder.Service.OpenTelemetryConfiguration.OltpExporterEnabled)
                {
                    b.AddOtlpExporter();
                }

                if (builder.Service.OpenTelemetryConfiguration.ConsoleExporterEnabled)
                {
                    b.AddConsoleExporter();
                }
            });
        }

        return builder;
    }
}