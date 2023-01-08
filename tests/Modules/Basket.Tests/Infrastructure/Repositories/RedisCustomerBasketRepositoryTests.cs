using Basket.Core.Model;
using Basket.Infrastructure.Repositories;
using Basket.Infrastructure.Serialization;
using Basket.Tests.Fixture;

using Shouldly;

namespace Basket.Tests.Infrastructure.Repositories;

[Collection(nameof(RedisFixtureCollection))]
public class RedisCustomerBasketRepositoryTests
{
    
    private readonly RedisFixture _redisFixture;

    public RedisCustomerBasketRepositoryTests(RedisFixture redisFixture)
    {
        _redisFixture = redisFixture;
    }

    [Fact]
    public async Task TestWhenCustomerBasketNotExists_ShouldReturnNull()
    {
        // Arrange
        var customerId = CustomerId.New();
        var deserializer = new SystemTextRedisObjectDeserializer();
        var repo = new RedisCustomerBasketRepository(_redisFixture.RedisConnection, deserializer);
        
        // Act

        var result = await repo.Find(customerId);

        result.ShouldBeNull();
    }
}