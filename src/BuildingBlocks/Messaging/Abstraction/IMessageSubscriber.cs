namespace Messaging.Abstraction;

public interface IMessageSubscriber<T> where T : IMessage
{
    Task Handle(T message, CancellationToken cancellationToken = default);
}

