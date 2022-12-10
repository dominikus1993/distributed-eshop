using System.Diagnostics.CodeAnalysis;

namespace Messaging.Abstraction;

public interface IMessageContext
{
    
}

public interface IMessagePublisher<T> where T : IMessage
{
    Task Publish([NotNull] T message, IMessageContext ctx, CancellationToken cancellationToken= default);
}