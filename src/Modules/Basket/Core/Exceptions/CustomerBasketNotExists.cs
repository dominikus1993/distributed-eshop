using System.Runtime.Serialization;

using Basket.Core.Model;

namespace Basket.Core.Exceptions;
public sealed class CustomerBasketNotExistsException : Exception
{
    private const string Message = "CustomerBasket not exists";
    
    public CustomerId CustomerId { get; }
    public CustomerBasketNotExistsException(CustomerId customerId) : base(Message)
    {
        CustomerId = customerId;
    }

    public CustomerBasketNotExistsException(Exception? innerException, CustomerId customerId) : base(Message, innerException)
    {
        CustomerId = customerId;
    }

    public override string ToString()
    {
        return $"{base.ToString()}, {nameof(CustomerId)}: {CustomerId}";
    }
}