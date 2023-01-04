using Catalog.Core.Model;
using Catalog.Core.Repository;
using Catalog.Infrastructure.DbContexts;
using Catalog.Infrastructure.Model;

using Mediator;

namespace Catalog.Infrastructure.Repositories;

public sealed class UnableToWriteRecordException : Exception
{
    public ProductId ProductId { get; }
    public UnableToWriteRecordException(ProductId productId) : base("can't write product to database")
    {
        ProductId = productId;
    }
    
    public UnableToWriteRecordException(ProductId productId, Exception innerException) : base("can't write product to database", innerException)
    {
        ProductId = productId;
    }

    public override string ToString()
    {
        return $"{base.ToString()}, {nameof(ProductId)}: {ProductId}";
    }
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
        return countOfInsertedRecords == 1 ? Unit.Value : new UnableToWriteRecordException(product.Id);
    }

    public Task<AddProductResult> AddProducts(IEnumerable<Product> products, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}