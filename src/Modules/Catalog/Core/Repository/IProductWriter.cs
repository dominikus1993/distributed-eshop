using Catalog.Core.Model;

using Mediator;

using OneOf;

namespace Catalog.Core.Repository;

[GenerateOneOf]
public sealed partial class AddProductResult : OneOfBase<Unit, Exception>
{
    
}

public interface IProductsWriter
{
    Task<AddProductResult> AddProduct(Product product, CancellationToken cancellationToken = default);
    
    Task<AddProductResult> AddProducts(IReadOnlyCollection<Product> products, CancellationToken cancellationToken = default);
}