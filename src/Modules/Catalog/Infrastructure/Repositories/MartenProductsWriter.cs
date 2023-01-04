using Catalog.Core.Model;
using Catalog.Core.Repository;
using Catalog.Infrastructure.DbContexts;
using Catalog.Infrastructure.Model;

using Mediator;

namespace Catalog.Infrastructure.Repositories;

public sealed class UnableToWriteRecordException : Exception
{
    
} 
public sealed class MartenProductsWriter : IProductsWriter
{
    private readonly ProductsDbContext _store;


    public MartenProductsWriter(ProductsDbContext store)
    {
        _store = store;
    }

    public async Task<AddProductResult> AddProduct(Product product, CancellationToken cancellationToken = default)
    {
        _store.Products.Add(new EfProduct(product));
        var countOfInsertedRecords = await _store.SaveChangesAsync(cancellationToken);
        return countOfInsertedRecords == 1 ? Unit.Value : new UnableToWriteRecordException();
    }

    public Task<AddProductResult> AddProducts(IEnumerable<Product> products, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}