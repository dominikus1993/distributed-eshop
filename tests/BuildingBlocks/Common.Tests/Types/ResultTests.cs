using AutoFixture.Xunit2;

using Common.Exceptions;
using Common.Types;

namespace Common.Tests.Types;

public sealed class ResultTests
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
    public void TestCaseWhenResultIsOk(string data)
    {
        var result = Result.Ok(data);
        
        var resultCase = result.Case;

        Assert.NotNull(resultCase);
        Assert.IsType<string>(resultCase);
        Assert.Equal(data, resultCase);
    }
    
    [Theory]
    [AutoData]
    public void TestCaseWhenResultIsError(Exception exception)
    {
        var result = Result.Failure<string>(exception);
        
        var resultCase = result.Case;

        Assert.NotNull(resultCase);
        Assert.IsType<Exception>(resultCase);
    }
    
    [Theory]
    [AutoData]
    public void TestCaseSwitchWhenResultIsOk(string data)
    {
        var result = Result.Ok(data);
        
        var resultCase = result.Case;

        switch (resultCase)
        {
            case string str:
                Assert.Equal(data, str);
                break;
            case Exception _:
                Assert.Fail();
                break;
            default:
                Assert.Fail();
                break;
        }
    }
    
    [Theory]
    [AutoData]
    public void TestCaseSwitchWhenResultIsError(Exception exception)
    {
        var result = Result.Failure<string>(exception);
        
        var resultCase = result.Case;

        switch (resultCase)
        {
            case string _:
                Assert.Fail();
                break;
            case Exception ex:
                Assert.Equivalent(ex, exception);
                break;
            default:
                Assert.Fail();
                break;
        }
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
        
        Assert.Equal(data, result.Value);
    }
    
    [Theory]
    [AutoData]
    public void TestResultErrorValueIfIsResultIsOk(string data)
    {
        var result = Result.Ok(data);

        var ex = Assert.Throws<ValueIsSuccessException<string>>(() => result.ErrorValue);
        Assert.NotNull(ex);
        Assert.Equivalent(data, ex.CurrentValue);
    }
    
    [Theory]
    [AutoData]
    public void TestResultErValueIfIsResultIsError(Exception error)
    {
        var result = Result.Failure<string>(error);
        
        var ex = Assert.Throws<ValueIsErrorException>(() => result.Value);
        Assert.NotNull(ex);
        Assert.Equivalent(error, ex.InnerException);
    }
    
    [Theory]
    [AutoData]
    public void TestResultErrorValueIfIsResultIsError(Exception error)
    {
        var result = Result.Failure<string>(error);

        var ex = result.ErrorValue;
        Assert.Equivalent(error, ex);
    }
    
    [Theory]
    [AutoData]
    public void TestResultToErrorIfResultIsError(Exception error)
    {
        var result = Result.Failure<string>(error);

        var ex = result.ToError<string, int>();
        Assert.False(ex.IsSuccess);
        Assert.Equivalent(error, ex.ErrorValue);
    }
    
    [Theory]
    [AutoData]
    public void TestResultToErrorIfResultIsSuccess(string value)
    {
        var result = Result.Ok(value);

        var ex = Assert.Throws<ValueIsSuccessException<string>>(() => result.ToError<string, int>());
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
        Assert.Equivalent(error, ex.ErrorValue);
    }
    
    [Theory]
    [AutoData]
    public void TestResultMapIfResultIsSuccess(int value, int newValue)
    {
        var result = Result.Ok(value);

        var subject = result.Map(_ => newValue);
        Assert.NotNull(subject);
        Assert.Equal(newValue, subject.Value);
    }
    
    
    [Theory]
    [AutoData]
    public void TestResultMatchIfResultIsError(Exception error)
    {
        var result = Result.Failure<int>(error);

        var ex = result.Match(_ => throw new InvalidOperationException("invalid test"), exception => exception);
        Assert.Equivalent(error, ex);
    }
    
    [Theory]
    [AutoData]
    public void TestResultMatchIfResultIsSuccess(int value, int newValue)
    {
        var result = Result.Ok(value);

        var subject = result.Match(_ => newValue, exception => throw exception);
        
        Assert.Equal(newValue, subject);
    }
    
    [Theory]
    [AutoData]
    public async Task TestResultMatchTaskIfResultIsError(Exception error)
    {
        var result = Result.Failure<int>(error);

        var ex = await result.MatchTask((_, _) => throw new InvalidOperationException("invalid test"), (exception, ct) => Task.FromResult(exception));
        Assert.Equivalent(error, ex);
    }
    
    [Theory]
    [AutoData]
    public async Task TestResultMatchTaskIfResultIsSuccess(int value, int newValue)
    {
        var result = Result.Ok(value);

        var subject = await result.MatchTask((_, _) => Task.FromResult(newValue), (exception, _) => throw exception);
        
        Assert.Equal(newValue, subject);
    }
    
    [Theory]
    [AutoData]
    public void TestResultMapWithClosureDependencyIfResultIsError(Exception error, int newValue)
    {
        var result = Result.Failure<int>(error);

        var ex = result.Map((x) => x + newValue);
        Assert.False(ex.IsSuccess);
        Assert.Equivalent(error, ex.ErrorValue);
    }
    
    [Theory]
    [AutoData]
    public void TestResultMapWithClosureDependencyIfResultIsSuccess(int value, int newValue)
    {
        var result = Result.Ok(value);

        var subject = result.Map((_) => newValue);
        Assert.NotNull(subject);
        Assert.Equal(newValue, subject.Value);
    }
}