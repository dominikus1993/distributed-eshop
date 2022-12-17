using Basket.Infrastructure.Model;
using Basket.Infrastructure.Serialization;

using Shouldly;

namespace Basket.Tests.Infrastructure.Serialization;

public class SystemTextRedisObjectDeserializerTests
{
    [Fact]
    public void TestSerializationAndDeserialization_ShouldSerializeObjectAndDeserializeToTheSameObject()
    {
        var basket = new RedisCustomerBasket()
        {
            CustomerId = Guid.NewGuid(),
            Items = new List<RedisBasketItem>() { new RedisBasketItem() { ItemId = 1, Quantity = 1 } }
        };
        var serializer = new SystemTextRedisObjectDeserializer();

        var serialized = serializer.Serialize(basket);

        var subject = serializer.Deserialize(serialized);

        subject.ShouldNotBeNull();
        
        subject.CustomerId.ShouldBe(basket.CustomerId);
        subject.Items.ShouldNotBeNull();
        subject.Items.ShouldNotBeEmpty();
        subject.Items.Count.ShouldBe(basket.Items.Count);
        subject.Items.ShouldContain(x => x.ItemId == 1 && x.Quantity == 1);
    }
}