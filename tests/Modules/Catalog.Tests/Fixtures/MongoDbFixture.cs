// namespace Catalog.Tests.Fixtures;
//
// public sealed class MongoDbFixture : IAsyncLifetime, IDisposable
// {
//     private readonly TestcontainerDatabaseConfiguration configuration =
//         new MongoDbTestcontainerConfiguration()
//         {
//             Database = MongoDbExtensions.MongoDbReceiptDatabaseName, Username = "mongo", Password = "mongo"
//         };
//
//     public MongoDbTestcontainer  Container { get; }
//     public IMongoClient Client { get; private set; }
//     public IMongoDatabase Database { get; private set; }
//     public MongoDbFixture()
//     {
//         this.Container = new TestcontainersBuilder<MongoDbTestcontainer>()
//             .WithDatabase(this.configuration)
//             .Build();
//     }
//
//     public async Task InitializeAsync()
//     {
//         await this.Container.StartAsync()
//             .ConfigureAwait(false);
//
//         Client = new MongoClient(Container.ConnectionString);
//         Database = Client.GetDatabase(Container.Database);
//         Database.SetupMongo();
//     }
//
//     public async Task DisposeAsync()
//     {
//         await this.Container.DisposeAsync()
//             .ConfigureAwait(false);
//     }
//
//     public void Dispose()
//     {
//         this.configuration.Dispose();
//     }
// }