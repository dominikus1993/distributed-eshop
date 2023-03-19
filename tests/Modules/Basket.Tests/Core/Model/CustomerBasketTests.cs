using AutoFixture.Xunit2;

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

        var subject = basket.Products;
        
        // Assert

        subject.ShouldBeEmpty();
    }
    
    [Theory]
    [InlineAutoData]
    public void TestAddItemWhenCustomerBasketIsEmpty(CustomerId customerId, Product item)
    {
        // Arrange 
        
        var basket = CustomerBasket.Empty(customerId).AddItem(item);
        
        // Act

        var subject = basket.Products;
        
        // Assert
        subject.IsEmpty.ShouldBeFalse();
        basket.CustomerId.ShouldBe(customerId);
        subject.ShouldNotBeEmpty();
        subject.ShouldContain(item);
    }
    
    [Theory]
    [InlineAutoData]
    public void TestAddItemWhenCustomerBasketIsNotEmptyAndItemExists(CustomerId customerId, Product item)
    {
        // Arrange 
        
        var basket = CustomerBasket.Empty(customerId).AddItem(item).AddItem(item);
        
        // Act

        var subject = basket.Products;
        
        // Assert
        subject.IsEmpty.ShouldBeFalse();
        subject.ShouldNotBeEmpty();
        subject.ShouldContain(item with { Quantity = new ItemQuantity(item.Quantity.Value * 2) });
    }
    
    
    [Theory]
    [InlineAutoData]
    public void TestRemoveItemWhenCustomerBasketIsNotEmptyAndItemExists__ShouldReturnNotEmptyBasket(ItemId itemId)
    {
        // Arrange 
        var item = new Product(itemId, new ItemQuantity(2));
        var basket = CustomerBasket.Empty(CustomerId.New()).AddItem(item).AddItem(item);
        
        // Act

        var subject = basket.RemoveItem(new Product(itemId, new ItemQuantity(1)));
        
        // Assert
        subject.IsEmpty.ShouldBeFalse();
        subject.Products.ShouldNotBeEmpty();
        subject.Products.ShouldContain(new Product(itemId, new ItemQuantity(3)));
    }
    
    [Theory]
    [InlineAutoData]
    public void TestRemoveItemWhenCustomerBasketIsNotEmptyAndItemExists__ShouldReturnEmptyBasket(ItemId itemId)
    {
        // Arrange 
        var item = new Product(itemId, new ItemQuantity(2));
        var basket = CustomerBasket.Empty(CustomerId.New()).AddItem(item).AddItem(item);
        
        // Act

        var subject = basket.RemoveItem(new Product(itemId, new ItemQuantity(4))).Products;
        
        // Assert
        subject.IsEmpty.ShouldBeTrue();
        subject.ShouldBeEmpty();
    }
}