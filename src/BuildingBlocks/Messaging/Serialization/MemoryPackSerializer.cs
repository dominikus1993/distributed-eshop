using System.Buffers;

using EasyNetQ;

namespace Messaging.Serialization;

public class MemoryPackSerializer : ISerializer
{
    public IMemoryOwner<byte> MessageToBytes(Type messageType, object message)
    {
        throw new NotImplementedException();
    }

    public object BytesToMessage(Type messageType, in ReadOnlyMemory<byte> bytes)
    {
        throw new NotImplementedException();
    }
}