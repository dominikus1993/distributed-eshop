using Basket.Core.Model;
using Basket.Core.RequestHandlers;
using Basket.Core.Requests;
using Basket.Infrastructure.Repositories;
using Basket.Infrastructure.Serialization;
using Basket.Tests.Fixture;

using Shouldly;

namespace Basket.Tests.Core.RequestHandlers;

[Collection(nameof(RedisFixtureCollectionTest))]
public class GetCustomerBasketHandlerTests
{
    private readonly RedisFixture _redisFixture;

    public GetCustomerBasketHandlerTests(RedisFixture redisFixture)
    {
        _redisFixture = redisFixture;
    }

    [Fact]
    public async Task TestWhenCustomerBasketNotExists_ShouldReturnNull()
    {
        // Arrange
        var customerId = CustomerId.New();
        var handler = new GetCustomerBasketHandler(_redisFixture.CustomerBasketReader);
        
        // Act

        var result = await handler.Handle(new GetCustomerBasket(customerId), CancellationToken.None);

        result.ShouldBeNull();
    }
}