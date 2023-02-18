using Basket.Core.Model;

using Shouldly;

using CustomerBasket = Basket.Core.Model.CustomerBasket;
using CustomerId = Basket.Core.Model.CustomerId;
using ItemId = Basket.Core.Model.ItemId;

namespace Basket.Tests.Core.Model;

public class CustomerBasketTests
{
    [Fact]
    public void TestGetItemsWhenCustomerBasketIsEmpty()
    {
        // Arrange 

        var basket = CustomerBasket.Empty(CustomerId.New());
        
        // Act

        var subject = basket.BasketItems;
        
        // Assert

        subject.ShouldBeEmpty();
    }
    
    [Fact]
    public void TestAddItemWhenCustomerBasketIsEmpty()
    {
        // Arrange 

        var item = new BasketItem(ItemId.New(), new ItemQuantity(2));
        var basket = CustomerBasket.Empty(CustomerId.New()).AddItem(item);
        
        // Act

        var subject = basket.BasketItems;
        
        // Assert
        subject.IsEmpty.ShouldBeFalse();
        subject.ShouldNotBeEmpty();
        subject.ShouldContain(item);
    }
    
    [Fact]
    public void TestAddItemWhenCustomerBasketIsNotEmptyAndItemExists()
    {
        // Arrange 

        var itemId = ItemId.New();
        var item = new BasketItem(itemId, new ItemQuantity(2));
        var basket = CustomerBasket.Empty(CustomerId.New()).AddItem(item).AddItem(item);
        
        // Act

        var subject = basket.BasketItems;
        
        // Assert
        subject.IsEmpty.ShouldBeFalse();
        subject.ShouldNotBeEmpty();
        subject.ShouldContain(new BasketItem(itemId, new ItemQuantity(4)));
    }
    
    
    [Fact]
    public void TestRemoveItemWhenCustomerBasketIsNotEmptyAndItemExists__ShouldReturnNotEmptyBasket()
    {
        // Arrange 

        var itemId = ItemId.New();
        var item = new BasketItem(itemId, new ItemQuantity(2));
        var basket = CustomerBasket.Empty(CustomerId.New()).AddItem(item).AddItem(item);
        
        // Act

        var subject = basket.RemoveItem(new BasketItem(itemId, new ItemQuantity(1))).BasketItems;
        
        // Assert
        subject.IsEmpty.ShouldBeFalse();
        subject.ShouldNotBeEmpty();
        subject.ShouldContain(new BasketItem(itemId, new ItemQuantity(3)));
    }
    
    [Fact]
    public void TestRemoveItemWhenCustomerBasketIsNotEmptyAndItemExists__ShouldReturnEmptyBasket()
    {
        // Arrange 

        var itemId = ItemId.New();
        var item = new BasketItem(itemId, new ItemQuantity(2));
        var basket = CustomerBasket.Empty(CustomerId.New()).AddItem(item).AddItem(item);
        
        // Act

        var subject = basket.RemoveItem(new BasketItem(itemId, new ItemQuantity(4))).BasketItems;
        
        // Assert
        subject.IsEmpty.ShouldBeTrue();
        subject.ShouldBeEmpty();
    }
}