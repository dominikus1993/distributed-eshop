namespace Messaging.Abstraction;

public interface IMessage
{
    public Guid Id { get; set; }
    public long Timestamp { get; set; }
}