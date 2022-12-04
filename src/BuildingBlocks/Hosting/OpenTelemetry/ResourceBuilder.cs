using OpenTelemetry.Resources;

namespace Hosting.OpenTelemetry;

public class ResourceBuilderExtensions
{
    private static IEnumerable<KeyValuePair<string, object>> GetAttributes(Service service, string envName)
    {
        yield return new KeyValuePair<string, object>("env", envName);
        if (service.Tags is { Count: > 0})
        {
            foreach ((string key, string value) in service.Tags)
            {
                yield return new KeyValuePair<string, object>(key, value);
            }
        }
    }
    
    public static ResourceBuilder GetResourceBuilder(Service service, string envName)
    {
        return ResourceBuilder.CreateDefault()
            .AddTelemetrySdk()
            .AddService(serviceName: service.Name, serviceVersion: service.Version, serviceInstanceId: Environment.MachineName )
            .AddAttributes(GetAttributes(service, envName));
    }
}