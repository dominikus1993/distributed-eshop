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

    public static readonly Result<Unit> UnitResult = new Result<Unit>.Success(Unit.Value);
}

public abstract partial class Result<T>
{
    private Result()
    {
        
    }
    
    public abstract bool IsSuccess { get; }
    public abstract T Value();
    public abstract Exception ErrorValue();
    public abstract Result<T2> ToError<T2>();
    public abstract Result<T2> Map<T2>(Func<T, T2> func);
    public abstract Result<T2> Map<T2, T3>(Func<T, T3, T2> func, T3 dependency);
    
    public abstract Result<T2> Bind<T2>(Func<T, Result<T2>> func);
    public abstract Result<T2> Bind<T2, T3>(Func<T, T3, Result<T2>> func, T3 dependency);
}
