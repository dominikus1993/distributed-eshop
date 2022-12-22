using Mediator;

using IMessage = Messaging.Abstraction.IMessage;

namespace Basket.Core.Events;

public sealed class BasketItemWasAdded : INotification, IMessage

{
    public required Guid Id { get; init; } = Guid.NewGuid();
}