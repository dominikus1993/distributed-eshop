using AutoFixture.Xunit2;

using Basket.Core.Model;
using Basket.Infrastructure.Repositories;
using Basket.Infrastructure.Serialization;
using Basket.Tests.Fixture;

using Shouldly;

namespace Basket.Tests.Infrastructure.Repositories;

[Collection(nameof(RedisFixtureCollectionTest))]
public class RedisCustomerBasketRepositoryTests
{
    
    private readonly RedisFixture _redisFixture;

    public RedisCustomerBasketRepositoryTests(RedisFixture redisFixture)
    {
        _redisFixture = redisFixture;
    }

    [Theory]
    [AutoData]
    internal async Task TestWhenCustomerBasketNotExists_ShouldReturnNull(CustomerId customerId, SystemTextRedisObjectDeserializer deserializer)
    {
        // Arrange
        var repo = new RedisCustomerBasketRepository(_redisFixture.RedisConnection, deserializer);
        
        // Act

        var result = await repo.Find(customerId);

        result.ShouldBeNull();
    }
}