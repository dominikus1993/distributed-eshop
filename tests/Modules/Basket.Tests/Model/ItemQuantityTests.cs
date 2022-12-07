using Basket.Model;

using Shouldly;

namespace Basket.Tests.Model;

public class ItemQuantityTests
{
    [Theory]
    [InlineData(4, 3, 1)]
    [InlineData(3, 3, 0)]
    [InlineData(3, 4, 0)]
    public void TestSubstractTwoItemQuantities(uint left, uint right, uint expected)
    {
        var leftItemQ = new ItemQuantity(left);
        var rightItemQ = new ItemQuantity(right);
        var subject = leftItemQ - rightItemQ;
        
        subject.ShouldBe(new ItemQuantity(expected));
    }
    
    [Theory]
    [InlineData(4, 3, 7)]
    [InlineData(3, 3, 6)]
    [InlineData(3, 40, 43)]
    public void TestAddTwoItemQuantities(uint left, uint right, uint expected)
    {
        var leftItemQ = new ItemQuantity(left);
        var rightItemQ = new ItemQuantity(right);
        var subject = leftItemQ + rightItemQ;
        
        subject.ShouldBe(new ItemQuantity(expected));
    }
}