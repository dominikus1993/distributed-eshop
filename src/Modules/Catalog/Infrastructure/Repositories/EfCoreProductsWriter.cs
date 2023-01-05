using System.Collections;
using System.Diagnostics.CodeAnalysis;

using Catalog.Core.Model;
using Catalog.Core.Repository;
using Catalog.Infrastructure.DbContexts;
using Catalog.Infrastructure.Model;

using Mediator;

namespace Catalog.Infrastructure.Repositories;

[Serializable]
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

[Serializable]
public sealed class UnableToWriteRecordsException : Exception
{
    public IEnumerable<ProductId> ProductIds { get; }
    public UnableToWriteRecordsException(IEnumerable<ProductId> productIds) : base("can't write product to database")
    {
        ProductIds = productIds;
    }
    
    public UnableToWriteRecordsException(IEnumerable<ProductId> productIds, Exception innerException) : base("can't write product to database", innerException)
    {
        ProductIds = productIds;
    }

    public override string ToString()
    {
        return $"{base.ToString()}, {nameof(ProductId)}: {string.Join(',', ProductIds)}";
    }
} 
public sealed class EfCoreProductsWriter : IProductsWriter
{
    private readonly ProductsDbContext _store;


    public EfCoreProductsWriter(ProductsDbContext store)
    {
        _store = store;
    }
    
    public async Task<AddProductResult> AddProduct(Product product, CancellationToken cancellationToken = default)
    {
        _store.Products.Add(new EfProduct(product));
        var countOfInsertedRecords = await SaveChangesAsync(cancellationToken);
        return countOfInsertedRecords == 1 ? Unit.Value : new UnableToWriteRecordException(product.Id);
    }

    public async Task<AddProductResult> AddProducts(IReadOnlyCollection<Product> products, CancellationToken cancellationToken = default)
    {
        foreach (var product in products)
        {
            _store.Products.Add(new EfProduct(product));
        }
        
        var countOfInsertedRecords = await SaveChangesAsync(cancellationToken);
        return countOfInsertedRecords == products.Count ? Unit.Value : new UnableToWriteRecordsException(products.Select(x => x.Id).ToList()); 
    }

    private Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _store.SaveChangesAsync(cancellationToken);
    }
}