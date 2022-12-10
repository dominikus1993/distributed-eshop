namespace Messaging.Abstraction;

public interface IMessageSubscriber
{
    Task Handle<T>(T message, CancellationToken cancellationToken = default) where T : IMessage;
}