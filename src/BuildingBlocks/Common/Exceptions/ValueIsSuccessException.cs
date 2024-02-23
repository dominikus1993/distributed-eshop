namespace Common.Exceptions;

public sealed class ValueIsSuccessException<T> : Exception
{
    public T CurrentValue { get; }
    
    public ValueIsSuccessException
        (T currentValue) : base("Value is Success")
    {

        CurrentValue = currentValue;
    }

    public override string ToString()
    {
        return $"{base.ToString()}, {nameof(CurrentValue)}: {CurrentValue}";
    }
}