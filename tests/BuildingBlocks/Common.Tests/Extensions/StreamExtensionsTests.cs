using System.Text;

using Common.Extensions;

namespace Common.Tests.Extensions;

public sealed class StreamExtensionsTests
{
    [Fact]
    public static void TestReadBytesWhenStreamIsNull()
    {
        using Stream stream = Stream.Null;

        var res = stream.ReadBytes(10);
        
        Assert.Empty(res);
    }
    
    [Fact]
    public static void TestReadBytesWhenStreamIsNulll()
    {
        Stream stream = null!;

        Assert.Throws<ArgumentNullException>(() => stream.ReadBytes(10));
    }
    
    [Fact]
    public static void TestReadBytesWhenCountIsNegative()
    {
        using Stream stream = Stream.Null;

        Assert.Throws<ArgumentOutOfRangeException>(() => stream.ReadBytes(-10));
    }
    
    [Fact]
    public static void TestReadBytesWhenCountIsZero()
    {
        using Stream stream = Stream.Null;

        var res = stream.ReadBytes(0);
        
        Assert.Empty(res);
    }
    
        
    [Fact]
    public static void TestWhenStreamIsShorterThanCount()
    {
        var data = "test"u8.ToArray();
        using Stream stream = new MemoryStream(data);

        var res = stream.ReadBytes(1000);
        
        Assert.NotEmpty(res);
        Assert.Equal(data.Length, res.Length);
        Assert.Equal(data, res);
    }
    
    [Fact]
    public static void TestWhenStreamIsEqualCount()
    {
        var data = "test"u8.ToArray();
        using Stream stream = new MemoryStream(data);

        var res = stream.ReadBytes(data.Length);
        
        Assert.NotEmpty(res);
        Assert.Equal(data.Length, res.Length);
        Assert.Equal(data, res);
    }
    
    [Fact]
    public static void TestWhenStreamIsLongerThanCount()
    {
        var data = "test jan pawel 3"u8.ToArray();
        using Stream stream = new MemoryStream(data);

        var res = stream.ReadBytes(data.Length - 1);
        
        Assert.NotEmpty(res);
        Assert.Equal(data.Length - 1, res.Length);
        Assert.NotEqual(data, res);
    }
}