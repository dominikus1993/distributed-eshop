using Basket.Model;

using Shouldly;

namespace Basket.Tests.Model;

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
}