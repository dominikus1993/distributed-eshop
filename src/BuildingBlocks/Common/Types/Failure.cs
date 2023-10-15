using Common.Exceptions;

namespace Common.Types;

public abstract partial class Result<T>
{
    public sealed class Failure: Result<T>
    {
        internal readonly Exception Error;
        
        public Failure(Exception error)
        {
            ArgumentNullException.ThrowIfNull(error);
            Error = error;
        }

        public override bool IsSuccess => false;
        public override T Value() => throw new ValueIsErrorException(Error);
        public override Exception ErrorValue() => Error;
        public override Result<T2> ToError<T2>()
        {
            return new Result<T2>.Failure(Error);
        }

        public override Result<T2> Map<T2>(Func<T, T2> func)
        {
            return ToError<T2>();
        }

        public override Result<T2> Map<T2, T3>(Func<T, T3, T2> func, T3 dependency)
        {
            return ToError<T2>();
        }

        public override Result<T2> Bind<T2>(Func<T, Result<T2>> func)
        {
            return ToError<T2>();
        }

        public override Result<T2> Bind<T2, T3>(Func<T, T3, Result<T2>> func, T3 dependency)
        {
            return ToError<T2>();
        }

        public void Deconstruct(out Exception error)
        {
            error = Error;
        }
    }
}