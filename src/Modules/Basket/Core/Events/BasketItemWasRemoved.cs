using System.Diagnostics.CodeAnalysis;

using Basket.Core.Model;

using Mediator;

using IMessage = Messaging.Abstraction.IMessage;

namespace Basket.Core.Events;

public sealed class BasketItemWasRemoved: INotification, IMessage

{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid CustomerId { get; set; }
    public required Guid ItemId { get; set; }
    public required uint Quantity { get; set; }

    [SetsRequiredMembers]
    public BasketItemWasRemoved(CustomerId customerId, BasketItem item)
    {
        ItemId = item.ItemId.Value;
        CustomerId = customerId.Value;
        Quantity = item.Quantity.Value;
    }
}