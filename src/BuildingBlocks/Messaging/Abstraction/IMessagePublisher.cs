using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Common.Types;

using OneOf;

namespace Messaging.Abstraction;

public interface IMessageContext
{
    ActivityContext? Context { get; init; }
}

[GenerateOneOf]
public sealed partial class PublishResult : OneOfBase<Exception, Unit>
{
    public bool IsSuccess => IsT1;

    public Exception Error => AsT0;
    internal static readonly PublishResult Ok = new PublishResult(Unit.Value);
}

public interface IMessagePublisher<in T> where T : IMessage
{
    Task<PublishResult> Publish([NotNull] T message, IMessageContext? ctx = null, CancellationToken cancellationToken= default);
}