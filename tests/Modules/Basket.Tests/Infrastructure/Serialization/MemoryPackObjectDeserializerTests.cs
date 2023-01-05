using Basket.Core.Model;
using Basket.Infrastructure.Model;
using Basket.Infrastructure.Serialization;

using Shouldly;

namespace Basket.Tests.Infrastructure.Serialization;

public class MemoryPackObjectDeserializerTests
{
    [Fact]
    public void TestSerializationAndDeserialization_ShouldSerializeObjectAndDeserializeToTheSameObject()
    {
        var itemId = ItemId.New();
        var basket = new RedisCustomerBasket()
        {
            CustomerId = Guid.NewGuid(),
            Items = new List<RedisBasketItem>() { new RedisBasketItem() { ItemId = itemId.Value, Quantity = 1 } }
        };
        var serializer = new MemoryPackObjectDeserializer();

        var serialized = serializer.Serialize(basket);

        var subject = serializer.Deserialize(serialized, out var result);
        
        subject.ShouldBeTrue();
        result.ShouldNotBeNull();
        
        result.CustomerId.ShouldBe(basket.CustomerId);
        result.Items.ShouldNotBeNull();
        result.Items.ShouldNotBeEmpty();
        result.Items.Count.ShouldBe(basket.Items.Count);
        result.Items.ShouldContain(x => x.ItemId == itemId.Value && x.Quantity == 1);
    }
}