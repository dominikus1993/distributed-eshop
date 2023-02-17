using Basket.Core.Events;
using Basket.Core.Exceptions;
using Basket.Core.Model;
using Basket.Core.RequestHandlers;
using Basket.Core.Requests;
using Basket.Tests.Fixture;

using Messaging.Abstraction;

using Moq;

using Shouldly;

namespace Basket.Tests.Core.RequestHandlers;

public class CheckoutCustomerBasketTests : IClassFixture<RedisFixture>
{
    private readonly RedisFixture _redisFixture;

    public CheckoutCustomerBasketTests(RedisFixture redisFixture)
    {
        _redisFixture = redisFixture;
    }

    [Fact]
    public async Task TestCheckoutWhenCustomerBasketIsActiveShouldNotExistsAfterCheckout()
    {
        var publisherMock = new Mock<IMessagePublisher<BasketItemWasAdded>>();
        publisherMock.Setup(x =>
            x.Publish(It.IsAny<BasketItemWasAdded>(), It.IsAny<IMessageContext>(), It.IsAny<CancellationToken>()));
        
        var customerId = CustomerId.New();
        var basketItem = new BasketItem(ItemId.New(), new ItemQuantity(1));
        var getCustomerBasket = new GetCustomerBasketHandler(_redisFixture.CustomerBasketReader);
        var addItemToCustomerBasketHandler = new AddItemToCustomerBasketHandler(_redisFixture.CustomerBasketReader, _redisFixture.CustomerBasketWriter, publisherMock.Object);
        var checkoutHandler =
            new CheckoutCustomerBasketHandler(_redisFixture.CustomerBasketReader, _redisFixture.CustomerBasketWriter);
        // Act
        await addItemToCustomerBasketHandler.Handle(new AddItemToCustomerBasket(customerId, basketItem), CancellationToken.None);

        await checkoutHandler.Handle(new CheckoutCustomerBasket(customerId), CancellationToken.None);
        
        var result = await getCustomerBasket.Handle(new GetCustomerBasket(customerId), CancellationToken.None);
        
        result.ShouldBeNull();
    }
    
    [Fact]
    public async Task TestCheckoutWhenCustomerBasketIsEmptyShouldThrowCustomerBasketNotExistsException()
    {
        var publisherMock = new Mock<IMessagePublisher<BasketItemWasAdded>>();
        publisherMock.Setup(x =>
            x.Publish(It.IsAny<BasketItemWasAdded>(), It.IsAny<IMessageContext>(), It.IsAny<CancellationToken>()));
        
        var customerId = CustomerId.New();
        var checkoutHandler =
            new CheckoutCustomerBasketHandler(_redisFixture.CustomerBasketReader, _redisFixture.CustomerBasketWriter);
        // Act
        
        await Assert.ThrowsAsync<CustomerBasketNotExists>(async () => await checkoutHandler.Handle(new CheckoutCustomerBasket(customerId), CancellationToken.None));
        
    }
}