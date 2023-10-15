using Basket.Core.Model;

using Common.Types;

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
    public static readonly UpdateBasketSuccess Instance;

    public override bool Equals(object? obj)
    {
        return obj is UpdateBasketSuccess;
    }

    public override int GetHashCode()
    {
        return 0;
    }

    public static bool operator ==(UpdateBasketSuccess left, UpdateBasketSuccess right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(UpdateBasketSuccess left, UpdateBasketSuccess right)
    {
        return !(left == right);
    }
}

public readonly struct RemoveBasketSuccess
{
    public static readonly RemoveBasketSuccess Instance = default;
    
    public override bool Equals(object? obj)
    {
        return obj is UpdateBasketSuccess;
    }

    public override int GetHashCode()
    {
        return 0;
    }

    public static bool operator ==(RemoveBasketSuccess left, RemoveBasketSuccess right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(RemoveBasketSuccess left, RemoveBasketSuccess right)
    {
        return !(left == right);
    }
}


public interface ICustomerBasketWriter
{
    Task<Result<UpdateBasketSuccess>> Update(CustomerBasket basket, CancellationToken cancellationToken = default);
    
    Task<Result<RemoveBasketSuccess>> Remove(CustomerId customerId, CancellationToken cancellationToken = default);
}