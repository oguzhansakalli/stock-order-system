using SharedKernel.Domain.Events;

namespace Orders.Domain.Events
{
    public sealed class OrderCreatedEvent : DomainEvent
    {
        public Guid OrderId { get; }
        public string OrderNumber { get; }
        public Guid CustomerId { get; }
        public decimal TotalAmount { get; }
        public int ItemCount { get; }
        public OrderCreatedEvent(Guid orderId, string orderNumber, Guid customerId, decimal totalAmount, int itemCount)
        {
            OrderId = orderId;
            OrderNumber = orderNumber;
            CustomerId = customerId;
            TotalAmount = totalAmount;
            ItemCount = itemCount;
        }
    }
}
