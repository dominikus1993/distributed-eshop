using System.Diagnostics.CodeAnalysis;

using Basket.Core.Model;

using Mediator;

using IMessage = Messaging.Abstraction.IMessage;

namespace Basket.Core.Events;

public sealed class BasketItemWasAdded : INotification, IMessage

{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required Guid CustomerId { get; init; }
    public required Guid ItemId { get; init; }
    public required uint Quantity { get; init; }

    [SetsRequiredMembers]
    public BasketItemWasAdded(CustomerId customerId, BasketItem item)
    {
        ItemId = item.ItemId.Value;
        CustomerId = customerId.Value;
        Quantity = item.Quantity.Value;
    }
}