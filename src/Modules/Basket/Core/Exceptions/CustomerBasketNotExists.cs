using Basket.Core.Model;

namespace Basket.Core.Exceptions;

public sealed class CustomerBasketNotExists : Exception
{
    private const string Message = "CustomerBasket not exists";
    
    public CustomerId CustomerId { get; }
    public CustomerBasketNotExists(CustomerId customerId) : base(Message)
    {
        CustomerId = customerId;
    }

    public CustomerBasketNotExists(Exception? innerException, CustomerId customerId) : base(Message, innerException)
    {
        CustomerId = customerId;
    }

    public override string ToString()
    {
        return $"{base.ToString()}, {nameof(CustomerId)}: {CustomerId}";
    }
}