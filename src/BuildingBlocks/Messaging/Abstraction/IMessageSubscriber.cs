using Common.Types;

namespace Messaging.Abstraction;

public interface IMessageSubscriber<in T> where T : IMessage
{
    Task<Result<Unit>> Handle(T message, CancellationToken cancellationToken = default);
}

