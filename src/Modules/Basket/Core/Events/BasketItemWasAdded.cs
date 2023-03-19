using System.Diagnostics.CodeAnalysis;

using Basket.Core.Model;

using Mediator;

using IMessage = Messaging.Abstraction.IMessage;

namespace Basket.Core.Events;

public sealed class BasketItemWasAdded : INotification, IMessage

{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid CustomerId { get; set; }
    public required Guid ItemId { get; set; }
    public required uint Quantity { get; set; }

    [SetsRequiredMembers]
    public BasketItemWasAdded(CustomerId customerId, Product item)
    {
        ItemId = item.ItemId.Value;
        CustomerId = customerId.Value;
        Quantity = item.Quantity.Value;
    }
}