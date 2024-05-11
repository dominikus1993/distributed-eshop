namespace Common.Exceptions;

public sealed class ValueIsSuccessException<T>(T currentValue) : Exception("Value is Success")
{
    public T CurrentValue { get; } = currentValue;

    public override string ToString()
    {
        return $"{base.ToString()}, {nameof(CurrentValue)}: {CurrentValue}";
    }
}