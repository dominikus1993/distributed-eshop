using System.Diagnostics.CodeAnalysis;

using Basket.Core.Model;

using Mediator;

using IMessage = Messaging.Abstraction.IMessage;

namespace Basket.Core.Events;

[method: SetsRequiredMembers]
public sealed class BasketItemWasAdded(CustomerId customerId, Product item) : INotification, IMessage

{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid CustomerId { get; set; } = customerId.Value;
    public required Guid ItemId { get; set; } = item.ItemId.Value;
    public required uint Quantity { get; set; } = item.Quantity.Value;
}