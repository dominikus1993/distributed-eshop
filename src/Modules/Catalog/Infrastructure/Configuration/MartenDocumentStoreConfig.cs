using Catalog.Infrastructure.Model;

using Marten;

using Weasel.Core;

namespace Catalog.Infrastructure.Configuration;

internal static class MartenDocumentStoreConfig
{
    public static Action<StoreOptions> Configure(string connectionString, bool isDev)
    {
        ArgumentNullException.ThrowIfNull(connectionString, nameof(connectionString));
        return options =>
        {
            options.Connection(connectionString);
            if (isDev)
            {
                options.AutoCreateSchemaObjects = AutoCreate.All;
            }

            options.Schema.For<MartenProduct>();
        };
    }
}