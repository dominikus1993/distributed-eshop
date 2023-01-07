using Catalog.Core.Repository;

using Shouldly;

using Xunit;

namespace Catalog.Tests.Core.Repository;

public class FilterQueryTests
{
    [Theory]
    [InlineData(1, 12, 0)]
    [InlineData(2, 12, 12)]
    [InlineData(3, 6, 12)]
    [InlineData(4, 20, 60)]
    public void SkipTests(int page, int pageSize, int expectedSkip)
    {
        var filter = new Filter() { Page = page, PageSize = pageSize };
        var subject = filter.Skip;
        
        subject.ShouldBe(expectedSkip);
    }
}