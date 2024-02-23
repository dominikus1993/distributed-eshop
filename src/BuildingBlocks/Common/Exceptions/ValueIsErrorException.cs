namespace Common.Exceptions;

public sealed class ValueIsErrorException : Exception
{
    private const string ErrorMessage = "Value is Error";
    
    public ValueIsErrorException(Exception innerException) : base(ErrorMessage, innerException)
    {
    }
}