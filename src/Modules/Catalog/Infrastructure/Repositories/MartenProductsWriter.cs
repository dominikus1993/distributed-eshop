using Catalog.Core.Model;
using Catalog.Core.Repository;

using Marten;

namespace Catalog.Infrastructure.Repositories;

internal sealed class MartenProductsWriter : IProductsWriter
{
    private readonly IDocumentStore _store;


    public MartenProductsWriter(IDocumentStore store)
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