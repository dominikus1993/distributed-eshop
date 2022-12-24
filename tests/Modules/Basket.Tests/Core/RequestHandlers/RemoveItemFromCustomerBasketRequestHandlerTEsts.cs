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

public class RemoveItemFromCustomerBasketRequestHandlerTests: IClassFixture<RedisFixture>
{
    private readonly RedisFixture _redisFixture;

    public RemoveItemFromCustomerBasketRequestHandlerTests(RedisFixture redisFixture)
    {
        _redisFixture = redisFixture;
    }

    [Fact]
    public async Task TestAddItemToEmptyBasket_AndRemoveIt_ShouldReturnEmptyBasket()
    {
        // Arrange
        var publisherMock = new Mock<IMessagePublisher<BasketItemWasAdded>>();
        publisherMock.Setup(x =>
            x.Publish(It.IsAny<BasketItemWasAdded>(), It.IsAny<IMessageContext>(), It.IsAny<CancellationToken>()));
        
        var publisherRemovedMock = new Mock<IMessagePublisher<BasketItemWasRemoved>>();
        publisherRemovedMock.Setup(x =>
            x.Publish(It.IsAny<BasketItemWasRemoved>(), It.IsAny<IMessageContext>(), It.IsAny<CancellationToken>()));
        
        var customerId = CustomerId.New();
        var basketItem = new BasketItem(new ItemId(1), new ItemQuantity(1));
        var deserializer = new SystemTextRedisObjectDeserializer();
        var repo = new RedisCustomerBasketRepository(_redisFixture.RedisConnection, deserializer);
        var getCustomerBasket = new GetCustomerBasketHandler(repo);
        var addhandler = new AddItemToCustomerBasketHandler(repo, repo, publisherMock.Object);
        var removeHandler = new RemoveItemFromCustomerBasketRequestHandler(repo, repo, publisherRemovedMock.Object);
        // Act
        await addhandler.Handle(new AddItemToCustomerBasket(customerId, basketItem), CancellationToken.None);
        await removeHandler.Handle(new RemoveItemFromCustomerBasket(customerId, basketItem), CancellationToken.None);
        
        var result = await getCustomerBasket.Handle(new GetCustomerBasket(customerId), CancellationToken.None);
        
        result.ShouldBeNull();
    }  
    
    [Fact]
    public async Task TestAddItemToEmptyBasket_AndRemoveOne_ShouldReturnBasketWithThisItemAndSmallerQuantity()
    {
        // Arrange
        var publisherMock = new Mock<IMessagePublisher<BasketItemWasAdded>>();
        publisherMock.Setup(x =>
            x.Publish(It.IsAny<BasketItemWasAdded>(), It.IsAny<IMessageContext>(), It.IsAny<CancellationToken>()));
        
        var publisherRemovedMock = new Mock<IMessagePublisher<BasketItemWasRemoved>>();
        publisherRemovedMock.Setup(x =>
            x.Publish(It.IsAny<BasketItemWasRemoved>(), It.IsAny<IMessageContext>(), It.IsAny<CancellationToken>()));
        
        var customerId = CustomerId.New();
        var basketItem = new BasketItem(new ItemId(1), new ItemQuantity(1));
        var deserializer = new SystemTextRedisObjectDeserializer();
        var repo = new RedisCustomerBasketRepository(_redisFixture.RedisConnection, deserializer);
        var getCustomerBasket = new GetCustomerBasketHandler(repo);
        var handler = new AddItemToCustomerBasketHandler(repo, repo, publisherMock.Object);
        var removeHandler = new RemoveItemFromCustomerBasketRequestHandler(repo, repo, publisherRemovedMock.Object);
        await handler.Handle(new AddItemToCustomerBasket(customerId, basketItem with { Quantity = new ItemQuantity(1) }), CancellationToken.None);
        
        // Act
        await handler.Handle(new AddItemToCustomerBasket(customerId, basketItem), CancellationToken.None);
        await removeHandler.Handle(new RemoveItemFromCustomerBasket(customerId, basketItem), CancellationToken.None);
        var result = await getCustomerBasket.Handle(new GetCustomerBasket(customerId), CancellationToken.None);
        
        result.ShouldNotBeNull();
        result.CustomerId.ShouldBe(customerId.Value);
        result.Items.ShouldNotBeEmpty();
        result.Items.Count.ShouldBe(1);
        result.Items.ShouldContain(x => x.ItemId == basketItem.ItemId.Value && x.Quantity == 1);
    }
    
}