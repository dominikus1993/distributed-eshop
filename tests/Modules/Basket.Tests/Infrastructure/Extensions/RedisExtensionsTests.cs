using Basket.Core.Model;
using Basket.Infrastructure.Extensions;

using Shouldly;

using StackExchange.Redis;

namespace Basket.Tests.Infrastructure.Extensions;

public class RedisExtensionsTests
{
    [Theory]
    [InlineData("some data", true, "some data")]
    [InlineData(null, false, null)]
    public void TestTryGetValue(string data, bool expectedResult, string expectedValue)
    {
        var redisValue = new RedisValue(data);
        var subject = redisValue.TryGetValue(out var result);
        
        subject.ShouldBe(expectedResult);
        result.ShouldBe(expectedValue);
    }
    
    [Theory]
    [InlineData("19bd31e5-9727-45f3-8dc8-40075c401dd2", "CustomerBasket:19bd31e5-9727-45f3-8dc8-40075c401dd2")]
    public void TestGetRedisKey(string uuid, string expectedValue)
    {
        var id = new CustomerId(Guid.Parse(uuid));
        var subject = id.ToRedisKey();
        
        subject.ToString().ShouldBe(expectedValue);
    }
}