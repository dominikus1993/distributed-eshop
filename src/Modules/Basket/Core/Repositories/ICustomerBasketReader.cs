using Basket.Core.Model;

using OneOf;

namespace Basket.Core.Repositories;

public interface ICustomerBasketReader
{
    Task<CustomerBasket?> Find(CustomerId customerId, CancellationToken cancellationToken = default);
}

public sealed class UpdateBasketException : Exception 
{
    public CustomerId CustomerId { get; }
    
    public UpdateBasketException(CustomerId customerId)
    {
        CustomerId = customerId;
    }

    public UpdateBasketException(CustomerId customerId, string? message) : base(message)
    {
        CustomerId = customerId;
    }

    public UpdateBasketException(CustomerId customerId, string? message, Exception? innerException) : base(message, innerException)
    {
        CustomerId = customerId;
    }

    public override string ToString()
    {
        return $"{base.ToString()}, {nameof(CustomerId)}: {CustomerId}";
    }
}

public sealed class RemoveBasketException : Exception 
{
    public CustomerId CustomerId { get; }
    
    public RemoveBasketException(CustomerId customerId)
    {
        CustomerId = customerId;
    }

    public RemoveBasketException(CustomerId customerId, string? message) : base(message)
    {
        CustomerId = customerId;
    }

    public RemoveBasketException(CustomerId customerId, string? message, Exception? innerException) : base(message, innerException)
    {
        CustomerId = customerId;
    }

    public override string ToString()
    {
        return $"{base.ToString()}, {nameof(CustomerId)}: {CustomerId}";
    }
}

public readonly struct UpdateBasketSuccess
{
    public static UpdateBasketSuccess Instance = new();
}

public readonly struct RemoveBasketSuccess
{
    public static RemoveBasketSuccess Instance = new();
}

[GenerateOneOf]
public sealed partial class UpdateCustomerBasketResult : OneOfBase<UpdateBasketSuccess, UpdateBasketException>
{
    
}

[GenerateOneOf]
public sealed partial class RemoveCustomerBasketResult : OneOfBase<RemoveBasketSuccess, RemoveBasketException>
{
    
}

public interface ICustomerBasketWriter
{
    Task<UpdateCustomerBasketResult> Update(CustomerBasket basket, CancellationToken cancellationToken = default);
    
    Task<RemoveCustomerBasketResult> Remove(CustomerId customerId, CancellationToken cancellationToken = default);
}