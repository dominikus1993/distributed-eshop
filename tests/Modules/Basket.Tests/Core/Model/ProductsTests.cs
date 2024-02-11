using AutoFixture.Xunit2;

using Basket.Core.Model;

namespace Basket.Tests.Core.Model;

public sealed class ProductsTests
{
    [Fact]
    public void CollectionInitializerWhenEmptyTests()
    {
        Products subject = [];
        
        Assert.Empty(subject);
        Assert.True(subject.IsEmpty);
    }
    
    [Theory]
    [AutoData]
    public void CollectionInitializerWhenNoEmptyTests(Product product)
    {
        Products subject = [product];
        
        Assert.False(subject.IsEmpty);
        Assert.Single(subject);
    }
}