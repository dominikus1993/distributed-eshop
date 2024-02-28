using Common.Exceptions;

namespace Common.Types;

using System;

public static class Result
{
    public static Result<T> Ok<T>(T ok)
    {
        return new Success<T>(ok);
    }
    
    public static Result<T> Failure<T>(Exception exception)
    {
        return new Failure<T>(exception);
    }

    public static readonly Result<Unit> UnitResult = new Success<Unit>(Unit.Value);

    public static Result<B> ToError<A, B>(this Result<A> res)
    {
        if (res.IsSuccess)
        {
            throw new ValueIsSuccessException<A>(res.Value);
        }
        
        return Failure<B>(res.ErrorValue);
    }
}

public abstract class Result<T>
{
    internal Result()
    {
        
    }
    
    public abstract bool IsSuccess { get; }
    public abstract T Value { get; }
    public abstract Exception ErrorValue { get; }
    public abstract Result<TRes> Map<TRes>(Func<T, TRes> map);
    public abstract TRes Match<TRes>(Func<T, TRes> ok, Func<Exception, TRes> error);
    public abstract void Match(Action<T> ok, Action<Exception> error);
    public abstract Task<TRes> MatchTask<TRes>(Func<T, CancellationToken, Task<TRes>> ok, Func<Exception, CancellationToken, Task<TRes>> func, CancellationToken token = default);
    public abstract Task MatchTask(Func<T, CancellationToken, Task> ok, Func<Exception, CancellationToken, Task> error, CancellationToken cancellationToken = default);
    public abstract object? Case { get; }
}

public sealed class Success<T>: Result<T>
{
    internal readonly T Ok;
        
    public Success(T ok)
    {
        ArgumentNullException.ThrowIfNull(ok);
        Ok = ok;
    }

    public override bool IsSuccess => true;
    public override T Value => Ok;
    public override Exception ErrorValue => throw new ValueIsSuccessException<T>(Ok);
    public override Result<TRes> Map<TRes>(Func<T, TRes> map)
    {
        ArgumentNullException.ThrowIfNull(map);
        var res = map(Ok);
        return new Success<TRes>(res);
    }

    public override TRes Match<TRes>(Func<T, TRes> ok, Func<Exception, TRes> error)
    {
        var res = ok(Ok);
        return res;
    }

    public override void Match(Action<T> ok, Action<Exception> error)
    {
        ok(Ok);
    }

    public override Task<TRes> MatchTask<TRes>(Func<T, CancellationToken, Task<TRes>> ok, Func<Exception, CancellationToken, Task<TRes>> error, CancellationToken token = default)
    {
        return ok(Ok, token);
    }

    public override Task MatchTask(Func<T, CancellationToken, Task> ok, Func<Exception, CancellationToken, Task> error, CancellationToken cancellationToken = default)
    {
        return ok(Ok, cancellationToken);
    }

    public override object? Case => Ok;

    public void Deconstruct(out T ok)
    {
        ok = Ok;
    }
}


internal sealed class Failure<T>: Result<T>
{
    internal readonly Exception Error;
        
    public Failure(Exception error)
    {
        ArgumentNullException.ThrowIfNull(error);
        Error = error;
    }

    public override bool IsSuccess => false;
    public override T Value => throw new ValueIsErrorException(Error);
    public override Exception ErrorValue => Error;
    public override Result<TRes> Map<TRes>(Func<T, TRes> map)
    {
        return new Failure<TRes>(Error);
    }

    public override TRes Match<TRes>(Func<T, TRes> ok, Func<Exception, TRes> error)
    {
        var res = error(Error);
        return res;
    }

    public override void Match(Action<T> ok, Action<Exception> error)
    {
        error(ErrorValue);
    }

    public override Task<TRes> MatchTask<TRes>(Func<T, CancellationToken, Task<TRes>> ok, Func<Exception, CancellationToken, Task<TRes>> error, CancellationToken token = default)
    {
        return error(Error, token);
    }

    public override Task MatchTask(Func<T, CancellationToken, Task> ok, Func<Exception, CancellationToken, Task> error, CancellationToken cancellationToken = default)
    {
        return error(Error, cancellationToken);
    }

    public override object? Case => Error;

    public void Deconstruct(out Exception error)
    {
        error = Error;
    }
}