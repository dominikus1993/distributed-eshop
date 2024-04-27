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
public sealed class AddItemToCustomerBasketHandlerTests
{
    private readonly RedisFixture _redisFixture;
    private readonly DateTimeOffset _dateTimeOffset = DateTimeOffset.Now;

    public AddItemToCustomerBasketHandlerTests(RedisFixture redisFixture)
    {
        _redisFixture = redisFixture;
    }

    [Theory]
    [AutoData]
    public async Task TestAddItemToEmptyBasket_ShouldReturnBasketWithOneItem(CustomerId customerId, Product basketItem)
    {
        // Arrange
        var publisherMock = Substitute.For<IMessagePublisher<BasketItemWasAdded>>();
        publisherMock.Publish(Arg.Any<BasketItemWasAdded>(), Arg.Any<IMessageContext>(), Arg.Any<CancellationToken>()).Returns(Result.UnitResult);
        
        var getCustomerBasket = new GetCustomerBasketHandler(_redisFixture.CustomerBasketReader);
        var handler = new AddItemToCustomerBasketHandler(_redisFixture.CustomerBasketReader, _redisFixture.CustomerBasketWriter, publisherMock, new FakeTimeProvider(_dateTimeOffset));
        // Act
        await handler.Handle(new AddItemToCustomerBasket(customerId, basketItem), CancellationToken.None);
        var result = await getCustomerBasket.Handle(new GetCustomerBasket(customerId), CancellationToken.None);
        
        result.ShouldNotBeNull();
        result.CustomerId.ShouldBe(customerId);
        result.Items.ShouldNotBeEmpty();
        result.Items.Count.ShouldBe(1);
        result.Items.ShouldContain(x => x.ItemId == basketItem.ItemId.Value && x.Quantity == basketItem.Quantity.Value);

        await publisherMock.Received(1).Publish(Arg.Any<BasketItemWasAdded>(), Arg.Any<IMessageContext>(), Arg.Any<CancellationToken>());
    }  
    
    [Fact]
    public async Task TestAddItemToNotEmptyBasket_ShouldReturnBasketWithTwoItems()
    {
        // Arrange
        var publisherMock = Substitute.For<IMessagePublisher<BasketItemWasAdded>>();
        publisherMock.Publish(Arg.Any<BasketItemWasAdded>(), Arg.Any<IMessageContext>(), Arg.Any<CancellationToken>()).Returns(Result.UnitResult);
        
        var customerId = CustomerId.New();
        var basketItem = new Product(ItemId.New(), new ItemQuantity(1));
        var getCustomerBasket = new GetCustomerBasketHandler(_redisFixture.CustomerBasketReader);
        var handler = new AddItemToCustomerBasketHandler(_redisFixture.CustomerBasketReader, _redisFixture.CustomerBasketWriter, publisherMock, new FakeTimeProvider(_dateTimeOffset));
        await handler.Handle(new AddItemToCustomerBasket(customerId, new Product(ItemId.New(), new ItemQuantity(2))), CancellationToken.None);
        
        // Act
        await handler.Handle(new AddItemToCustomerBasket(customerId, basketItem), CancellationToken.None);
        var result = await getCustomerBasket.Handle(new GetCustomerBasket(customerId), CancellationToken.None);
        
        result.ShouldNotBeNull();
        result.CustomerId.ShouldBe(customerId);
        result.Items.ShouldNotBeEmpty();
        result.Items.Count.ShouldBe(2);
        result.Items.ShouldContain(x => x.ItemId == basketItem.ItemId.Value && x.Quantity == basketItem.Quantity.Value);
        
        await publisherMock.Received(2).Publish(Arg.Any<BasketItemWasAdded>(), Arg.Any<IMessageContext>(), Arg.Any<CancellationToken>());
    }
}