using System.Buffers;

namespace Common.Extensions;


internal sealed class BytesStreamReader : IDisposable
{
    private readonly bool _disposeStreamAfterRead;
    private static readonly ArrayPool<byte> ArrayPool = ArrayPool<byte>.Shared;
    private readonly byte[] _bytes;
    private readonly Stream _stream;

    public BytesStreamReader(Stream stream, int bufferSize = 8, bool disposeStreamAfterRead = false)
    {
        _stream = stream;
        _disposeStreamAfterRead = disposeStreamAfterRead;
        _bytes = ArrayPool.Rent(bufferSize);
    }
    
    public bool SequenceEqual(byte[] arr)
    {
        var readBytes = _stream.ReadAtLeast(_bytes, _bytes.Length, false);
        var arrSpan = arr.AsSpan();
        var bytesSpan = _bytes.AsSpan(0, readBytes);
        return arrSpan.SequenceEqual(bytesSpan);
    }
    
    public void Dispose()
    {
        if (_disposeStreamAfterRead)
        {
            _stream.Dispose();
        }
        ArrayPool.Return(_bytes);
    }
    
}

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