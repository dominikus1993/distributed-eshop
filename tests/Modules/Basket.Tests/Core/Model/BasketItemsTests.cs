using Basket.Core.Model;

using Shouldly;

using ItemId = Basket.Core.Model.ItemId;

namespace Basket.Tests.Core.Model;

public class BasketItemsTests
{
    [Fact]
    public void TestBasketItemsRemoveOrDecreaseItemWhenHasNoElements()
    {
        var itemId = ItemId.New();
        var q = new ItemQuantity(11);
        var itemId2 = ItemId.New();
        var items = BasketItems.Empty.AddItems(new[] { new BasketItem(itemId, q), new BasketItem(itemId2, q) });

        var subject = items.RemoveOrDecreaseItem(new BasketItem(itemId, q));
        
        subject.IsEmpty.ShouldBeFalse();
        subject.ShouldNotBeEmpty();
        subject.Count().ShouldBe(1);
        subject.ShouldContain(new BasketItem(itemId2, q));
    }
    
    [Fact]
    public void TestBasketItemsRemoveOrDecreaseItemWhenHasElementAfterRemoveOrDecrease()
    {
        var itemId = ItemId.New();
        var q = new ItemQuantity(11);
        var itemId2 = ItemId.New();
        var items = BasketItems.Empty.AddItems(new[] { new BasketItem(itemId, q), new BasketItem(itemId2, q) });

        var subject = items.RemoveOrDecreaseItem(new BasketItem(itemId, new ItemQuantity(5)));
        
        subject.IsEmpty.ShouldBeFalse();
        subject.ShouldNotBeEmpty();
        subject.Count().ShouldBe(2);
        subject.ShouldContain(new BasketItem(itemId, new ItemQuantity(6)));
        subject.ShouldContain(new BasketItem(itemId2, q));
    }
}