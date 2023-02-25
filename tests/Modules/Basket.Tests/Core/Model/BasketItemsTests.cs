using AutoFixture.Xunit2;

using Basket.Core.Model;

using Shouldly;

using ItemId = Basket.Core.Model.ItemId;

namespace Basket.Tests.Core.Model;

public class BasketItemsTests
{
    [Theory]
    [InlineAutoData]
    public void TestBasketItemsRemoveOrDecreaseItemWhenHasNoElements(BasketItem item1, BasketItem item2)
    {
        var items = BasketItems.Empty.AddItems(new[] { item1, item2 });

        var subject = items.RemoveOrDecreaseItem(item1);
        
        subject.IsEmpty.ShouldBeFalse();
        subject.ShouldNotBeEmpty();
        subject.Count().ShouldBe(1);
        subject.ShouldContain(item2);
    }
    
    [Theory]
    [InlineAutoData]
    public void TestBasketItemsRemoveOrDecreaseItemWhenHasElementAfterRemoveOrDecrease(ItemId item1Id, BasketItem item2)
    {
        var q = new ItemQuantity(10);
        var items = BasketItems.Empty.AddItems(new[] { new BasketItem(item1Id, q), item2 });

        var subject = items.RemoveOrDecreaseItem(new BasketItem(item1Id, new ItemQuantity(2)));
        
        subject.IsEmpty.ShouldBeFalse();
        subject.ShouldNotBeEmpty();
        subject.Count().ShouldBe(2);
        subject.ShouldContain(new BasketItem(item1Id, new ItemQuantity(8)));
        subject.ShouldContain(item2);
    }
}