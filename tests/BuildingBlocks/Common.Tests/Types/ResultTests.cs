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
    
    [Theory]
    [AutoData]
    public void TestResultToErrorIfResultIsError(Exception error)
    {
        var result = Result.Failure<string>(error);

        var ex = result.ToError<int>();
        Assert.False(ex.IsSuccess);
        Assert.Equivalent(error, ex.ErrorValue());
    }
    
    [Theory]
    [AutoData]
    public void TestResultToErrorIfResultIsSuccess(string value)
    {
        var result = Result.Ok(value);

        var ex = Assert.Throws<ValueIsSuccessException<string>>(() => result.ToError<int>());
        Assert.NotNull(ex);
        Assert.Equal(value, ex.CurrentValue);
    }
    
    [Theory]
    [AutoData]
    public void TestResultMapIfResultIsError(Exception error)
    {
        var result = Result.Failure<int>(error);

        var ex = result.Map((x) => x + 10);
        Assert.False(ex.IsSuccess);
        Assert.Equivalent(error, ex.ErrorValue());
    }
    
    [Theory]
    [AutoData]
    public void TestResultMapIfResultIsSuccess(int value, int newValue)
    {
        var result = Result.Ok(value);

        var subject = result.Map(_ => newValue);
        Assert.NotNull(subject);
        Assert.Equal(newValue, subject.Value());
    }
    
    [Theory]
    [AutoData]
    public void TestResultMapWithClosureDependencyIfResultIsError(Exception error, int newValue)
    {
        var result = Result.Failure<int>(error);

        var ex = result.Map((x, val) => x + val, newValue);
        Assert.False(ex.IsSuccess);
        Assert.Equivalent(error, ex.ErrorValue());
    }
    
    [Theory]
    [AutoData]
    public void TestResultMapWithClosureDependencyIfResultIsSuccess(int value, int newValue)
    {
        var result = Result.Ok(value);

        var subject = result.Map((_, newVal) => newVal, newValue);
        Assert.NotNull(subject);
        Assert.Equal(newValue, subject.Value());
    }
}