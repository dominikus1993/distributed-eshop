using Basket.Core.Model;

using Shouldly;

using ItemId = Basket.Core.Model.ItemId;

namespace Basket.Tests.Core.Model;

public class BasketItemsTests
{
    [Fact]
    public void TestBasketItemsRemoveOrDecreaseItemWhenHasNoElements()
    {
        var itemId = new ItemId(1);
        var q = new ItemQuantity(11);

        var items = new BasketItems(new[] { new BasketItem(itemId, q), new BasketItem(new ItemId(1212), q) });

        var subject = items.RemoveOrDecreaseItem(new BasketItem(itemId, q));
        
        subject.IsEmpty.ShouldBeFalse();
        subject.Items.ShouldNotBeEmpty();
        subject.Items.Count.ShouldBe(1);
        subject.Items.ShouldContain(new BasketItem(new ItemId(1212), q));
    }
    
    [Fact]
    public void TestBasketItemsRemoveOrDecreaseItemWhenHasElementAfterRemoveOrDecrease()
    {
        var itemId = new ItemId(1);
        var q = new ItemQuantity(11);

        var items = new BasketItems(new[] { new BasketItem(itemId, q), new BasketItem(new ItemId(1212), q) });

        var subject = items.RemoveOrDecreaseItem(new BasketItem(itemId, new ItemQuantity(5)));
        
        subject.IsEmpty.ShouldBeFalse();
        subject.Items.ShouldNotBeEmpty();
        subject.Items.Count.ShouldBe(2);
        subject.Items.ShouldContain(new BasketItem(itemId, new ItemQuantity(6)));
        subject.Items.ShouldContain(new BasketItem(new ItemId(1212), q));
    }
}