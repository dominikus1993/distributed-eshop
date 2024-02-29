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

        ReadBytes(stream, ref buffer);
        
        return buffer;
    }
    
    
    public static void ReadBytes(this Stream stream, ref byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (stream.Length == 0)
        {
            buffer = [];
            return;
        }

        if (buffer.Length == 0)
        {
            return;
        }
        
        var readBytes = stream.ReadAtLeast(buffer, buffer.Length, false);

        if (readBytes < buffer.Length)
        {
            Array.Resize(ref buffer, readBytes);
        }
    }
}