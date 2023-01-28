using System.Collections;
using System.Diagnostics.CodeAnalysis;

using Catalog.Core.Model;
using Catalog.Core.Repository;
using Catalog.Infrastructure.DbContexts;
using Catalog.Infrastructure.Model;

using Mediator;

using Microsoft.EntityFrameworkCore;

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
    
    private UnableToWriteRecordException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
        
    }

    public override string ToString()
    {
        return $"{base.ToString()}, {nameof(ProductId)}: {ProductId}";
    }
} 

[Serializable]
public sealed class UnableToWriteRecordsException : Exception
{
    public IEnumerable<ProductId>? ProductIds { get; }
    public UnableToWriteRecordsException(IEnumerable<ProductId> productIds) : base("can't write product to database")
    {
        ProductIds = productIds;
    }
    
    public UnableToWriteRecordsException(IEnumerable<ProductId> productIds, Exception innerException) : base("can't write product to database", innerException)
    {
        ProductIds = productIds;
    }
    
    private UnableToWriteRecordsException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
        
    }

    public override string ToString()
    {
        return $"{base.ToString()}, {nameof(ProductId)}: {string.Join(',', ProductIds ?? Enumerable.Empty<ProductId>())}";
    }
} 
public sealed class EfCoreProductsWriter : IProductsWriter
{
    private readonly IDbContextFactory<ProductsDbContext> _storeFactory;


    public EfCoreProductsWriter(IDbContextFactory<ProductsDbContext> store)
    {
        _storeFactory = store;
    }
    
    public async Task<AddProductResult> AddProduct(Product product, CancellationToken cancellationToken = default)
    {
        await using var context = await _storeFactory.CreateDbContextAsync(cancellationToken);
        var countOfInsertedRecords = await context.Products.Upsert(new EfProduct(product)).On(p => p.ProductId).RunAsync(cancellationToken);
        return countOfInsertedRecords == 1 ? Unit.Value : new UnableToWriteRecordException(product.Id);
    }

    public async Task<AddProductResult> AddProducts(IReadOnlyCollection<Product> products, CancellationToken cancellationToken = default)
    {
        await using var context = await _storeFactory.CreateDbContextAsync(cancellationToken);
        var records = products.Select(product => new EfProduct(product));

        var countOfInsertedRecords = await context.Products.UpsertRange(records).On(product => product.ProductId).RunAsync(cancellationToken);
        return countOfInsertedRecords == products.Count ? Unit.Value : new UnableToWriteRecordsException(products.Select(x => x.Id).ToList()); 
    }

    public async Task RemoveAllProducts(CancellationToken cancellationToken = default)
    {
        await using var context = await _storeFactory.CreateDbContextAsync(cancellationToken);
        context.Products.RemoveRange(context.Products);
        await context.SaveChangesAsync(cancellationToken);
    }
}