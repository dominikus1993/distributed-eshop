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
    
    [Theory]
    [AutoData]
    public void AddItemTests(Product product)
    {
        Products subject = [];
        
        var result = subject.AddItem(product);
        
        Assert.Single(result);
        Assert.Contains(result, x => x == product);
    }
    
    [Fact]
    public void AddNullItemTests()
    {
        Products subject = [];
        
        Assert.Throws<ArgumentNullException>(() => subject.AddItem(null!));
    }
    
    
    [Theory]
    [AutoData]
    public void MapOneItemItemsTests(Product product)
    {
        Products subject = [];
        
        var result = subject.AddItem(product);
        
        Assert.Single(result);
        
        var mapped = result.MapItems(x => x.Quantity);
        
        Assert.Single(mapped);
    }
    
    [Fact]
    public void MapEmptyItemsTests()
    {
        Products subject = [];
        
        var mapped = subject.MapItems(x => x.Quantity);
        
        Assert.Empty(mapped);
    }
    
    [Theory]
    [AutoData]
    public void MapItemsTests(Product[] products)
    {
        Products subject = [];

        foreach (var p in products)
        {
            subject = subject.AddItem(p);
        }
        
        var mapped = subject.MapItems(x => x.Quantity);
        
        Assert.Equal(products.Length, mapped.Count);
    }
}