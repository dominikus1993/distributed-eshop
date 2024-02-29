namespace Common.Extensions;

public static class StreamExtensions
{
    public static byte[] ReadBytes(this Stream stream, int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        ArgumentNullException.ThrowIfNull(stream);
        if (stream.Length == 0)
        {
            return [];
        }

        if (count == 0)
        {
            return [];
        }
        
        var buffer = new byte[count];
        var readBytes = stream.ReadAtLeast(buffer, count, false);

        if (readBytes < count)
        {
            Array.Resize(ref buffer, readBytes);
        }
        return buffer;
    }
}