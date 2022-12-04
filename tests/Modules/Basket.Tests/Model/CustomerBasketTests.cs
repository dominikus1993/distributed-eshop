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

        var subject = basket.Items;
        
        // Assert

        subject.ShouldBeEmpty();
    }
}