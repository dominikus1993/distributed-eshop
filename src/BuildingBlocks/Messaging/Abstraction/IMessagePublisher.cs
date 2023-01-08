using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Messaging.Abstraction;

public interface IMessageContext
{
    ActivityContext? Context { get; init; }
}

public interface IMessagePublisher<in T> where T : IMessage
{
    Task Publish([NotNull] T message, IMessageContext? ctx = null, CancellationToken cancellationToken= default);
}