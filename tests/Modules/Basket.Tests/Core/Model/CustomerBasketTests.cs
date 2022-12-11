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

        var subject = basket.GetItems();
        
        // Assert

        subject.Items.ShouldBeEmpty();
    }
    
    [Fact]
    public void TestAddItemWhenCustomerBasketIsEmpty()
    {
        // Arrange 

        var item = new BasketItem(new ItemId(1), new ItemQuantity(2));
        var basket = CustomerBasket.Empty(CustomerId.New()).AddItem(item);
        
        // Act

        var subject = basket.GetItems();
        
        // Assert
        subject.IsEmpty.ShouldBeFalse();
        subject.Items.ShouldNotBeEmpty();
        subject.Items.ShouldContain(item);
    }
    
    [Fact]
    public void TestAddItemWhenCustomerBasketIsNotEmptyAndItemExists()
    {
        // Arrange 

        var item = new BasketItem(new ItemId(1), new ItemQuantity(2));
        var basket = CustomerBasket.Empty(CustomerId.New()).AddItem(item).AddItem(item);
        
        // Act

        var subject = basket.GetItems();
        
        // Assert
        subject.IsEmpty.ShouldBeFalse();
        subject.Items.ShouldNotBeEmpty();
        subject.Items.ShouldContain(new BasketItem(new ItemId(1), new ItemQuantity(4)));
    }
    
    
    [Fact]
    public void TestRemoveItemWhenCustomerBasketIsNotEmptyAndItemExists__ShouldReturnNotEmptyBasket()
    {
        // Arrange 

        var itemId = new ItemId(1);
        var item = new BasketItem(itemId, new ItemQuantity(2));
        var basket = CustomerBasket.Empty(CustomerId.New()).AddItem(item).AddItem(item);
        
        // Act

        var subject = basket.RemoveItem(new BasketItem(itemId, new ItemQuantity(1))).GetItems();
        
        // Assert
        subject.IsEmpty.ShouldBeFalse();
        subject.Items.ShouldNotBeEmpty();
        subject.Items.ShouldContain(new BasketItem(itemId, new ItemQuantity(3)));
    }
    
    [Fact]
    public void TestRemoveItemWhenCustomerBasketIsNotEmptyAndItemExists__ShouldReturnEmptyBasket()
    {
        // Arrange 

        var itemId = new ItemId(1);
        var item = new BasketItem(itemId, new ItemQuantity(2));
        var basket = CustomerBasket.Empty(CustomerId.New()).AddItem(item).AddItem(item);
        
        // Act

        var subject = basket.RemoveItem(new BasketItem(itemId, new ItemQuantity(4))).GetItems();
        
        // Assert
        subject.IsEmpty.ShouldBeTrue();
        subject.Items.ShouldBeEmpty();
    }
}