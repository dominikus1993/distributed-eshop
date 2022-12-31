using Catalog.Core.Model;

namespace Catalog.Core.Repository;

public interface IProductReader
{
    Task<Product?> GetById(ProductId id, CancellationToken cancellationToken = default);
    
    Task<Product?> GetByIds(IEnumerable<ProductId> id, CancellationToken cancellationToken = default);
}