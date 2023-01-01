using Catalog.Core.Model;
using Catalog.Core.Repository;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories;

public sealed class MartenProductsWriter : IProductsWriter
{
    private readonly IMongoDatabase _store;


    public MartenProductsWriter(IMongoDatabase store)
    {
        _store = store;
    }

    public Task<AddProductResult> AddProduct(Product product, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<AddProductResult> AddProducts(IEnumerable<Product> products, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}