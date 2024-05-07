using AutoFixture.Xunit2;

using Basket.Core.Model;
using Basket.Infrastructure.Model;
using Basket.Infrastructure.Serialization;

using FluentAssertions;

using Shouldly;

using StackExchange.Redis;

namespace Basket.Tests.Infrastructure.Serialization;

public class MemoryPackObjectDeserializerTests
{
    [Theory]
    [AutoData]
    internal void TestSerializationAndDeserialization_ShouldSerializeObjectAndDeserializeToTheSameObject(RedisCustomerBasket basket, MemoryPackObjectDeserializer serializer)
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
    
    [Theory]
    [AutoData]
    internal void TestNullDeserialization_ShouldDeserializeNull(MemoryPackObjectDeserializer serializer)
    {
        RedisValue value = RedisValue.Null;
        var subject = serializer.Deserialize(value, out var result);
        
        subject.Should().BeFalse();
        result.Should().BeNull();
    }
    
    [Theory]
    [AutoData]
    internal void TestSerializationWhenNull_ShouldThrowArgumentNullException(MemoryPackObjectDeserializer serializer)
    {
        Assert.Throws<ArgumentNullException>(() => serializer.Serialize(null!));
    }
}