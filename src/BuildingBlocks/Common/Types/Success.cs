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

        public void Deconstruct(out T ok)
        {
            ok = Ok;
        }
    }
}