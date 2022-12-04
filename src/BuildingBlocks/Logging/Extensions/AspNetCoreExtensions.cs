using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using OpenTelemetry.Logs;

namespace Logging.Extensions;

public static class AspNetCoreExtensions
{
    public static WebApplicationBuilder UseOpenTelemetryLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.AddOpenTelemetry(b =>
        {
            b.IncludeFormattedMessage = true;
            b.IncludeScopes = true;
            b.ParseStateValues = true;
            var value = builder.Configuration.Get<bool>("dsadsasad");
            if (builder.Configuration.GetValue<bool>(""))
            {
                
            }
            b.AddOtlpExporter();
        });

    }
}