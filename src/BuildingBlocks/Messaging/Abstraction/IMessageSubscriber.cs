namespace Messaging.Abstraction;

public interface IMessageSubscriber<in T> where T : IMessage
{
    Task Handle(T message, CancellationToken cancellationToken = default);
}

