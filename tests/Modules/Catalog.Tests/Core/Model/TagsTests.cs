using Catalog.Core.Model;

namespace Catalog.Tests.Core.Model;

public sealed class TagsTests
{
    [Fact]
    public void TestCollectionBuilderWhenValueIsEmpty()
    {
        IEnumerable<Tag> value = Enumerable.Empty<Tag>();
        Tags tags = [..value];
        
        Assert.Empty(tags);
    }
    
    [Fact]
    public void TestCollectionBuilderWhenValueIsNotEmpty()
    {
        var tag = new Tag("Test");
        IEnumerable<Tag> value = [tag];
        Tags tags = [..value];
        
        Assert.NotEmpty(tags);
        Assert.Single(tags, tag);
    }
}