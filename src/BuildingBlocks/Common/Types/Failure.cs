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

        public void Deconstruct(out Exception error)
        {
            error = Error;
        }
    }
}