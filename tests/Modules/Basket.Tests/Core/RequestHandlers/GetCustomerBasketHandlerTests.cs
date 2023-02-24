using AutoFixture.Xunit2;

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
    private readonly GetCustomerBasketHandler _getCustomerBasketHandler;

    public GetCustomerBasketHandlerTests(RedisFixture redisFixture)
    {
        _redisFixture = redisFixture;
        _getCustomerBasketHandler = new GetCustomerBasketHandler(_redisFixture.CustomerBasketReader);
    }

    [Theory]
    [InlineAutoData]
    public async Task TestWhenCustomerBasketNotExists_ShouldReturnNull(CustomerId customerId)
    {
        // Act

        var result = await _getCustomerBasketHandler.Handle(new GetCustomerBasket(customerId), CancellationToken.None);

        result.ShouldBeNull();
    }
}