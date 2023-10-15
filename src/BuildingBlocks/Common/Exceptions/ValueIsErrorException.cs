namespace Common.Exceptions;

public sealed class ValueIsErrorException : Exception
{
    private const string Message = "Value is Error";
    
    public ValueIsErrorException(Exception innerException) : base(Message, innerException)
    {
    }
}