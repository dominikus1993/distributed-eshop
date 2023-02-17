using Basket.Core.Events;
using Basket.Core.Model;
using Basket.Core.RequestHandlers;
using Basket.Core.Requests;
using Basket.Infrastructure.Repositories;
using Basket.Infrastructure.Serialization;
using Basket.Tests.Fixture;

using Messaging.Abstraction;

using Moq;

using Shouldly;

namespace Basket.Tests.Core.RequestHandlers;

[Collection(nameof(RedisFixtureCollectionTest))]
public class AddItemToCustomerBasketHandlerTests
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
        var publisherMock = new Mock<IMessagePublisher<BasketItemWasAdded>>();
        publisherMock.Setup(x =>
            x.Publish(It.IsAny<BasketItemWasAdded>(), It.IsAny<IMessageContext>(), It.IsAny<CancellationToken>()));
        
        var customerId = CustomerId.New();
        var basketItem = new BasketItem(ItemId.New(), new ItemQuantity(1));
        var getCustomerBasket = new GetCustomerBasketHandler(_redisFixture.CustomerBasketReader);
        var handler = new AddItemToCustomerBasketHandler(_redisFixture.CustomerBasketReader, _redisFixture.CustomerBasketWriter, publisherMock.Object);
        // Act
        await handler.Handle(new AddItemToCustomerBasket(customerId, basketItem), CancellationToken.None);
        var result = await getCustomerBasket.Handle(new GetCustomerBasket(customerId), CancellationToken.None);
        
        result.ShouldNotBeNull();
        result.CustomerId.ShouldBe(customerId.Value);
        result.Items.ShouldNotBeEmpty();
        result.Items.Count.ShouldBe(1);
        result.Items.ShouldContain(x => x.ItemId == basketItem.ItemId.Value && x.Quantity == basketItem.Quantity.Value);
        
        publisherMock.Verify(x => x.Publish(It.IsAny<BasketItemWasAdded>(), null, It.IsAny<CancellationToken>()), Times.Exactly(1));
    }  
    
    [Fact]
    public async Task TestAddItemToNotEmptyBasket_ShouldReturnBasketWithTwoItems()
    {
        // Arrange
        var publisherMock = new Mock<IMessagePublisher<BasketItemWasAdded>>();
        publisherMock.Setup(x =>
            x.Publish(It.IsAny<BasketItemWasAdded>(), It.IsAny<IMessageContext>(), It.IsAny<CancellationToken>()));
        
        var customerId = CustomerId.New();
        var basketItem = new BasketItem(ItemId.New(), new ItemQuantity(1));
        var getCustomerBasket = new GetCustomerBasketHandler(_redisFixture.CustomerBasketReader);
        var handler = new AddItemToCustomerBasketHandler(_redisFixture.CustomerBasketReader, _redisFixture.CustomerBasketWriter, publisherMock.Object);
        await handler.Handle(new AddItemToCustomerBasket(customerId, new BasketItem(ItemId.New(), new ItemQuantity(2))), CancellationToken.None);
        
        // Act
        await handler.Handle(new AddItemToCustomerBasket(customerId, basketItem), CancellationToken.None);
        var result = await getCustomerBasket.Handle(new GetCustomerBasket(customerId), CancellationToken.None);
        
        result.ShouldNotBeNull();
        result.CustomerId.ShouldBe(customerId.Value);
        result.Items.ShouldNotBeEmpty();
        result.Items.Count.ShouldBe(2);
        result.Items.ShouldContain(x => x.ItemId == basketItem.ItemId.Value && x.Quantity == basketItem.Quantity.Value);
        
        publisherMock.Verify(x => x.Publish(It.IsAny<BasketItemWasAdded>(), null, It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
}