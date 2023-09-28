using Catalog.Core.Model;

using Mediator;

using OneOf;

namespace Catalog.Core.Repository;

[GenerateOneOf]
public sealed partial class AddProductResult : OneOfBase<Unit, Exception>
{
    public static AddProductResult Ok() => new AddProductResult(Unit.Value);
    public static AddProductResult Error(Exception exception) => new AddProductResult(exception);
}

public interface IProductsWriter
{
    Task<AddProductResult> AddProduct(Product product, CancellationToken cancellationToken = default);
    
    Task<AddProductResult> AddProducts(IEnumerable<Product> products, CancellationToken cancellationToken = default);

    Task RemoveAllProducts(CancellationToken cancellationToken = default);
}