namespace Common.Types;

using System;

public static class Result
{
    public static Result<T> Ok<T>(T ok)
    {
        return new Result<T>.Success(ok);
    }
    
    public static Result<T> Failure<T>(Exception exception)
    {
        return new Result<T>.Failure(exception);
    }
}

public abstract partial class Result<T>
{
    private Result()
    {
        
    }
    
    public abstract bool IsSuccess { get; }
    public abstract T Value();
    public abstract Exception ErrorValue();
}
