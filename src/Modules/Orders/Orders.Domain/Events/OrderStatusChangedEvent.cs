using Orders.Domain.Enums;
using SharedKernel.Domain.Events;

namespace Orders.Domain.Events
{
    public sealed class OrderStatusChangedEvent : DomainEvent
    {
        public Guid OrderId { get; }
        public string OrderNumber { get; }
        public OrderStatus OldStatus { get; }
        public OrderStatus NewStatus { get; }
        public DateTime ChangedAt { get; }

        public OrderStatusChangedEvent(Guid orderId, string orderNumber, OrderStatus oldStatus, OrderStatus newStatus)
        {
            OrderId = orderId;
            OrderNumber = orderNumber;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            ChangedAt = DateTime.UtcNow;
        }
    }
}
