using System.Runtime.Serialization;

namespace Common.Exceptions;

public sealed class ValueIsSuccessException<T> : Exception
{
    public T CurrentValue { get; }
    
    public ValueIsSuccessException
        (T currentValue) : base("Value is Success")
    {

        CurrentValue = currentValue;
    }
    
    private ValueIsSuccessException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        CurrentValue = (T)info.GetValue(nameof(CurrentValue), typeof(T))!;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(CurrentValue), CurrentValue);
    }

    public override string ToString()
    {
        return $"{base.ToString()}, {nameof(CurrentValue)}: {CurrentValue}";
    }
}