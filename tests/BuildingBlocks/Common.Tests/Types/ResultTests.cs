using AutoFixture.Xunit2;

using Common.Exceptions;
using Common.Types;

namespace Common.Tests.Types;

public class ResultTests
{
    [Theory]
    [AutoData]
    public void TestResultIsSuccessIfIsResultIsOk(string data)
    {
        var result = Result.Ok(data);
        
        Assert.True(result.IsSuccess);
    }
    
    [Theory]
    [AutoData]
    public void TestResultIsSuccessIfIsResultIsFailure(Exception exception)
    {
        var result = Result.Failure<string>(exception);
        
        Assert.False(result.IsSuccess);
    }
    
    [Theory]
    [AutoData]
    public void TestResultValueIfIsResultIsOk(string data)
    {
        var result = Result.Ok(data);
        
        Assert.Equal(data, result.Value());
    }
    
    [Theory]
    [AutoData]
    public void TestResultErrorValueIfIsResultIsOk(string data)
    {
        var result = Result.Ok(data);

        var ex = Assert.Throws<ValueIsSuccessException<string>>(() => result.ErrorValue());
        Assert.NotNull(ex);
        Assert.Equivalent(data, ex.CurrentValue);
    }
    
    [Theory]
    [AutoData]
    public void TestResultErValueIfIsResultIsError(Exception error)
    {
        var result = Result.Failure<string>(error);
        
        var ex = Assert.Throws<ValueIsErrorException>(() => result.Value());
        Assert.NotNull(ex);
        Assert.Equivalent(error, ex.InnerException);
    }
    
    [Theory]
    [AutoData]
    public void TestResultErrorValueIfIsResultIsError(Exception error)
    {
        var result = Result.Failure<string>(error);

        var ex = result.ErrorValue();
        Assert.Equivalent(error, ex);
    }
}