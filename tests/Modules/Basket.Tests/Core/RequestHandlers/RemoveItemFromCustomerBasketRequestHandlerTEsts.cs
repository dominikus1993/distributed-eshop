using AutoFixture.Xunit2;

using Basket.Core.Events;
using Basket.Core.Model;
using Basket.Core.RequestHandlers;
using Basket.Core.Requests;
using Basket.Infrastructure.Repositories;
using Basket.Infrastructure.Serialization;
using Basket.Tests.Fixture;

using Common.Types;

using Messaging.Abstraction;

using Microsoft.Extensions.Time.Testing;

using NSubstitute;

using Shouldly;

namespace Basket.Tests.Core.RequestHandlers;

[Collection(nameof(RedisFixtureCollectionTest))]
public class RemoveItemFromCustomerBasketRequestHandlerTests
{
    private readonly RedisFixture _redisFixture;
    private readonly DateTimeOffset _dateTimeOffset = DateTimeOffset.Now;
    
    public RemoveItemFromCustomerBasketRequestHandlerTests(RedisFixture redisFixture)
    {
        _redisFixture = redisFixture;
    }

    [AutoData]
    [Theory]
    public async Task TestAddItemToEmptyBasket_AndRemoveIt_ShouldReturnEmptyBasket(CustomerId customerId)
    {
        // Arrange
        var publisherMock = Substitute.For<IMessagePublisher<BasketItemWasAdded>>();
        publisherMock.Publish(Arg.Any<BasketItemWasAdded>(), Arg.Any<IMessageContext>(), Arg.Any<CancellationToken>()).Returns(Result.UnitResult);
        
        var publisherRemovedMock =Substitute.For<IMessagePublisher<BasketItemWasRemoved>>();
        publisherRemovedMock.Publish(Arg.Any<BasketItemWasRemoved>(), Arg.Any<IMessageContext>(), Arg.Any<CancellationToken>()).Returns(Result.UnitResult);
        
        var basketItem = new Product(ItemId.New(), new ItemQuantity(1));
        var getCustomerBasket = new GetCustomerBasketHandler(_redisFixture.CustomerBasketReader);
        var addhandler = new AddItemToCustomerBasketHandler(_redisFixture.CustomerBasketReader, _redisFixture.CustomerBasketWriter, publisherMock, new FakeTimeProvider(_dateTimeOffset));
        var removeHandler = new RemoveItemFromCustomerBasketRequestHandler(_redisFixture.CustomerBasketReader, _redisFixture.CustomerBasketWriter, publisherRemovedMock, new FakeTimeProvider(_dateTimeOffset));
        // Act
        await addhandler.Handle(new AddItemToCustomerBasket(customerId, basketItem), CancellationToken.None);
        await removeHandler.Handle(new RemoveItemFromCustomerBasket(customerId, basketItem), CancellationToken.None);
        
        var result = await getCustomerBasket.Handle(new GetCustomerBasket(customerId), CancellationToken.None);
        
        result.ShouldBeNull();
        
        await publisherRemovedMock.Received(1).Publish(Arg.Any<BasketItemWasRemoved>(), Arg.Any<IMessageContext>(), Arg.Any<CancellationToken>());
    }  
    
    [Fact]
    public async Task TestAddItemToEmptyBasket_AndRemoveOne_ShouldReturnBasketWithThisItemAndSmallerQuantity()
    {
        // Arrange
        var publisherMock = Substitute.For<IMessagePublisher<BasketItemWasAdded>>();
        publisherMock.Publish(Arg.Any<BasketItemWasAdded>(), Arg.Any<IMessageContext>(), Arg.Any<CancellationToken>()).Returns(Result.UnitResult);
        
        var publisherRemovedMock =Substitute.For<IMessagePublisher<BasketItemWasRemoved>>();
        publisherRemovedMock.Publish(Arg.Any<BasketItemWasRemoved>(), Arg.Any<IMessageContext>(), Arg.Any<CancellationToken>()).Returns(Result.UnitResult);
        
        var customerId = CustomerId.New();
        var basketItem = new Product(ItemId.New(), new ItemQuantity(1));
        var getCustomerBasket = new GetCustomerBasketHandler(_redisFixture.CustomerBasketReader);
        var addhandler = new AddItemToCustomerBasketHandler(_redisFixture.CustomerBasketReader, _redisFixture.CustomerBasketWriter, publisherMock, new FakeTimeProvider(_dateTimeOffset));
        var removeHandler = new RemoveItemFromCustomerBasketRequestHandler(_redisFixture.CustomerBasketReader, _redisFixture.CustomerBasketWriter, publisherRemovedMock, new FakeTimeProvider(_dateTimeOffset));
        await addhandler.Handle(new AddItemToCustomerBasket(customerId, basketItem with { Quantity = new ItemQuantity(1) }), CancellationToken.None);
        
        // Act
        await addhandler.Handle(new AddItemToCustomerBasket(customerId, basketItem), CancellationToken.None);
        await removeHandler.Handle(new RemoveItemFromCustomerBasket(customerId, basketItem), CancellationToken.None);
        var result = await getCustomerBasket.Handle(new GetCustomerBasket(customerId), CancellationToken.None);
        
        result.ShouldNotBeNull();
        result.CustomerId.ShouldBe(customerId);
        result.Items.ShouldNotBeEmpty();
        result.Items.Count.ShouldBe(1);
        result.Items.ShouldContain(x => x.ItemId == basketItem.ItemId.Value && x.Quantity == 1);
        await publisherRemovedMock.Received(1).Publish(Arg.Any<BasketItemWasRemoved>(), Arg.Any<IMessageContext>(), Arg.Any<CancellationToken>());
    }
    
}