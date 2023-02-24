using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

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
    private readonly AutoFixture.Fixture _autoFixture = new();
    private readonly GetCustomerBasketHandler _getCustomerBasketHandler;
    private readonly AddItemToCustomerBasketHandler _addItemToCustomerBasketHandler;
    private readonly CheckoutCustomerBasketHandler _checkoutCustomerBasketHandler;
    
    public CheckoutCustomerBasketTests(RedisFixture redisFixture)
    {
        _redisFixture = redisFixture;
        _autoFixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
        _getCustomerBasketHandler = new GetCustomerBasketHandler(_redisFixture.CustomerBasketReader);
        _addItemToCustomerBasketHandler = new AddItemToCustomerBasketHandler(_redisFixture.CustomerBasketReader,
            _redisFixture.CustomerBasketWriter, _autoFixture.Create<IMessagePublisher<BasketItemWasAdded>>());
        _checkoutCustomerBasketHandler = new CheckoutCustomerBasketHandler(_redisFixture.CustomerBasketReader, _redisFixture.CustomerBasketWriter);
    }

    [Theory]
    [InlineAutoData]
    public async Task TestCheckoutWhenCustomerBasketIsActiveShouldNotExistsAfterCheckout(CustomerId customerId, BasketItem basketItem)
    {
       
        // Arrange 
        
        await _addItemToCustomerBasketHandler.Handle(new AddItemToCustomerBasket(customerId, basketItem), CancellationToken.None);
        
        // Act

        await _checkoutCustomerBasketHandler.Handle(new CheckoutCustomerBasket(customerId), CancellationToken.None);
        
        var result = await _getCustomerBasketHandler.Handle(new GetCustomerBasket(customerId), CancellationToken.None);
        
        result.ShouldBeNull();
    }
    
    [Theory]
    [InlineAutoData]
    public async Task TestCheckoutWhenCustomerBasketIsEmptyShouldThrowCustomerBasketNotExistsException(CustomerId customerId)
    {
        // Act
        await Assert.ThrowsAsync<CustomerBasketNotExists>(async () => await _checkoutCustomerBasketHandler.Handle(new CheckoutCustomerBasket(customerId), CancellationToken.None));
    }
}