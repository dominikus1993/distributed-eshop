using OneOf;

namespace Basket.Model;

public readonly record struct CustomerId(Guid Value);

public readonly record struct EmptyBasket(CustomerId CustomerId0); 

[GenerateOneOf]
public partial class CustomerBasket: OneOfBase<EmptyBasket>
{
    
}