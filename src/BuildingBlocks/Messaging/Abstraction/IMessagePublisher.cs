using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Common.Types;

using OneOf;

namespace Messaging.Abstraction;

public interface IMessageContext
{
    ActivityContext? Context { get; init; }
}

public interface IMessagePublisher<in T> where T : IMessage
{
    Task<Result<Unit>> Publish([NotNull] T message, IMessageContext? ctx = null, CancellationToken cancellationToken= default);
}