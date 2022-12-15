using Basket.Core.Model;
using Basket.Core.RequestHandlers;
using Basket.Core.Requests;
using Basket.Infrastructure.Repositories;
using Basket.Infrastructure.Serialization;
using Basket.Tests.Fixture;

using Shouldly;

namespace Basket.Tests.Core.RequestHandlers;

public class AddItemToCustomerBasketHandlerTests: IClassFixture<RedisFixture>
{
    private readonly RedisFixture _redisFixture;

    public AddItemToCustomerBasketHandlerTests(RedisFixture redisFixture)
    {
        _redisFixture = redisFixture;
    }

    [Fact]
    public async Task TestAddItemToEmptyBasket_ShouldReturnBasketWithOneItem()
    {
        // Arrange
        var customerId = CustomerId.New();
        var basketItem = new BasketItem(new ItemId(1), new ItemQuantity(1));
        var deserializer = new SystemTextRedisObjectDeserializer();
        var repo = new RedisCustomerBasketRepository(_redisFixture.RedisConnection, deserializer);
        var getCustomerBasket = new GetCustomerBasketHandler(repo);
        var handler = new AddItemToCustomerBasketHandler(repo, repo);
        // Act
        await handler.Handle(new AddItemToCustomerBasket(customerId, basketItem), CancellationToken.None);
        var result = await getCustomerBasket.Handle(new GetCustomerBasket(customerId), CancellationToken.None);
        
        result.ShouldNotBeNull();
        result.CustomerId.ShouldBe(customerId.Value);
        result.Items.ShouldNotBeEmpty();
        result.Items.Count.ShouldBe(1);
        result.Items.ShouldContain(x => x.ItemId == basketItem.ItemId.Value && x.Quantity == basketItem.Quantity.Value);
    }  
    
    [Fact]
    public async Task TestAddItemToNotEmptyBasket_ShouldReturnBasketWithTwoItems()
    {
        // Arrange
        var customerId = CustomerId.New();
        var basketItem = new BasketItem(new ItemId(1), new ItemQuantity(1));
        var deserializer = new SystemTextRedisObjectDeserializer();
        var repo = new RedisCustomerBasketRepository(_redisFixture.RedisConnection, deserializer);
        var getCustomerBasket = new GetCustomerBasketHandler(repo);
        var handler = new AddItemToCustomerBasketHandler(repo, repo);
        await handler.Handle(new AddItemToCustomerBasket(customerId, new BasketItem(new ItemId(2), new ItemQuantity(2))), CancellationToken.None);
        
        // Act
        await handler.Handle(new AddItemToCustomerBasket(customerId, basketItem), CancellationToken.None);
        var result = await getCustomerBasket.Handle(new GetCustomerBasket(customerId), CancellationToken.None);
        
        result.ShouldNotBeNull();
        result.CustomerId.ShouldBe(customerId.Value);
        result.Items.ShouldNotBeEmpty();
        result.Items.Count.ShouldBe(2);
        result.Items.ShouldContain(x => x.ItemId == basketItem.ItemId.Value && x.Quantity == basketItem.Quantity.Value);
    }
}