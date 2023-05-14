using System.Runtime.Serialization;

using Basket.Core.Model;

namespace Basket.Core.Exceptions;

[Serializable]
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

    private CustomerBasketNotExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        CustomerId = (CustomerId)info.GetValue(nameof(CustomerId), typeof(CustomerId))!;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(CustomerId), CustomerId);
    }

    public override string ToString()
    {
        return $"{base.ToString()}, {nameof(CustomerId)}: {CustomerId}";
    }
}