using System.Diagnostics.CodeAnalysis;

using Hosting.OpenTelemetry;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

using OpenTelemetry.Logs;

using Telemetry.OpenTelemetry;

namespace Logging.Extensions;

public static class AspNetCoreExtensions
{
    public static WebApplicationBuilder AddOpenTelemetryLogging(this WebApplicationBuilder builder, [NotNull]Service service)
    {
        ArgumentNullException.ThrowIfNull(service);
        
        if (service.OpenTelemetryConfiguration.OpenTelemetryLoggingEnabled)
        {
            builder.Logging.AddOpenTelemetry(b =>
            {
                b.IncludeFormattedMessage = true;
                b.IncludeScopes = true;
                b.ParseStateValues = true;
                b.AttachLogsToActivityEvent();
                b.SetResourceBuilder(
                    ResourceBuilderExtensions.GetResourceBuilder(service, builder.Environment.EnvironmentName));
            
                if (service.OpenTelemetryConfiguration.OltpExporterEnabled)
                {
                    b.AddOtlpExporter();
                }

                if (service.OpenTelemetryConfiguration.ConsoleExporterEnabled)
                {
                    b.AddConsoleExporter();
                }
            });
        }
        return builder;
    }
}