using Common.Exceptions;

namespace Common.Types;

public abstract partial class Result<T>
{
    public sealed class Success: Result<T>
    {
        internal readonly T Ok;
        
        public Success(T ok)
        {
            ArgumentNullException.ThrowIfNull(ok);
            Ok = ok;
        }

        public override bool IsSuccess => true;
        public override T Value() => Ok;
        public override Exception ErrorValue() => throw new ValueIsSuccessException<T>(Ok);
        public override Result<T2> ToError<T2>()
        {
            throw new ValueIsSuccessException<T>(Ok);
        }

        public override Result<T2> Map<T2>(Func<T, T2> func)
        {
            ArgumentNullException.ThrowIfNull(func);
            var value = func(Ok);
            return new Result<T2>.Success(value);
        }

        public override Result<T2> Map<T2, T3>(Func<T, T3, T2> func, T3 dependency)
        {
            ArgumentNullException.ThrowIfNull(func);
            var value = func(Ok, dependency);
            return new Result<T2>.Success(value);
        }

        public override Result<T2> Bind<T2>(Func<T, Result<T2>> func)
        {
            ArgumentNullException.ThrowIfNull(func);
            var result = func(Ok);
            return result;
        }

        public override Result<T2> Bind<T2, T3>(Func<T, T3, Result<T2>> func, T3 dependency)
        {
            ArgumentNullException.ThrowIfNull(func);
            var result = func(Ok, dependency);
            return result;
        }

        public void Deconstruct(out T ok)
        {
            ok = Ok;
        }
    }
}