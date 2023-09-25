using AutoFixture.Xunit2;

using Basket.Infrastructure.Model;
using Basket.Infrastructure.Serialization;

using FluentAssertions;

namespace Basket.Tests.Infrastructure.Serialization;

public class SystemTextObjectDeserializerTests
{
    [Theory]
    [AutoData]
    internal void TestSerializationAndDeserialization_ShouldSerializeObjectAndDeserializeToTheSameObject(RedisCustomerBasket basket, SystemTextRedisObjectDeserializer serializer)
    {
        var serialized = serializer.Serialize(basket);

        var subject = serializer.Deserialize(serialized, out var result);
        
        subject.Should().BeTrue();
        result.Should().NotBeNull();
        
        result!.CustomerId.Should().Be(basket.CustomerId);
        result.Items.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
        result.Items.Should().HaveCount(basket.Items.Count);
        foreach (var item in basket.Items)
        {
            result.Items.Should().ContainEquivalentOf(item);
        }
    }
}