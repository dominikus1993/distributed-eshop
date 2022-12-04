using OneOf;

namespace Basket.Model;

public readonly record struct CustomerId(Guid Value);

public readonly record struct EmptyBasket(CustomerId CustomerId0); 

[GenerateOneOf]
public sealed partial class CustomerBasket: OneOfBase<EmptyBasket>
{
    
}