using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Hosting.HealthChecks;

public static class EndpointExtensions
{
    public static WebApplication MapHealthCheckEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/healthz",
            new HealthCheckOptions() { Predicate = _ => true, ResponseWriter = (context, report) =>
                UIResponseWriter.WriteHealthCheckUIResponse(context, report)
            });
        app.MapHealthChecks("/ping",
            new HealthCheckOptions() { Predicate = r => r.Name.Contains("self"), ResponseWriter =
                (context, report) => PongWriteResponse(context, report), });

        return app;
    }
    
    static Task PongWriteResponse(HttpContext httpContext,
        HealthReport _)
    {
        httpContext.Response.StatusCode = 200;
        httpContext.Response.ContentType = "application/json";
        return httpContext.Response.WriteAsync("{\"pong\": \"message\"}");
    }
}