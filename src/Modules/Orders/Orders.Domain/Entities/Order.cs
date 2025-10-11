using Orders.Domain.Enums;
using Orders.Domain.Events;
using Orders.Domain.ValueObjects;
using SharedKernel.Domain.Entities;

namespace Orders.Domain.Entities
{
    public class Order : BaseEntity
    {
        public OrderNumber OrderNumber { get; private set; }
        public Guid CustomerId { get; private set; }
        public string CustomerName { get; private set; }
        public OrderStatus Status { get; private set; }
        public decimal TotalAmount { get; private set; }
        public string? Notes { get; private set; }

        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        private Order() { } // For EF Core
        public static Order Create(
            Guid customerId,
            string customerName,
            string? notes,
            Guid tenantId)
        {
            if (customerId == Guid.Empty)
                throw new ArgumentException("Customer ID cannot be empty", nameof(customerId));

            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentException("Customer name cannot be empty", nameof(customerName));

            var order = new Order
            {
                OrderNumber = OrderNumber.Generate(),
                CustomerId = customerId,
                CustomerName = customerName,
                Status = OrderStatus.Pending,
                TotalAmount = 0,
                Notes = notes
            };

            order.SetTenantId(tenantId);

            return order;
        }
        public void AddItem(OrderItem item)
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException($"Cannot add items to order with status {Status}");

            if (item == null)
                throw new ArgumentNullException(nameof(item));

            // Check if product already exists in order
            var existingItem = _items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existingItem != null)
            {
                // Update quantity instead of adding duplicate
                existingItem.UpdateQuantity(existingItem.Quantity + item.Quantity);
            }
            else
            {
                _items.Add(item);
            }

            RecalculateTotalAmount();
        }

        public void RemoveItem(Guid orderItemId)
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException($"Cannot remove items from order with status {Status}");

            var item = _items.FirstOrDefault(i => i.Id == orderItemId);
            if (item != null)
            {
                _items.Remove(item);
                RecalculateTotalAmount();
            }
        }

        public void UpdateItemQuantity(Guid orderItemId, int newQuantity)
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException($"Cannot update items in order with status {Status}");

            var item = _items.FirstOrDefault(i => i.Id == orderItemId);
            if (item == null)
                throw new InvalidOperationException("Order item not found");

            item.UpdateQuantity(newQuantity);
            RecalculateTotalAmount();
        }

        public void Confirm()
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException($"Only pending orders can be confirmed. Current status: {Status}");

            if (!_items.Any())
                throw new InvalidOperationException("Cannot confirm order without items");

            var oldStatus = Status;
            Status = OrderStatus.Confirmed;

            AddDomainEvent(new OrderCreatedEvent(Id, OrderNumber, CustomerId, TotalAmount, _items.Count));
            AddDomainEvent(new OrderStatusChangedEvent(Id, OrderNumber, oldStatus, Status));
        }

        public void StartProcessing()
        {
            if (Status != OrderStatus.Confirmed)
                throw new InvalidOperationException($"Only confirmed orders can be processed. Current status: {Status}");

            var oldStatus = Status;
            Status = OrderStatus.Processing;
            AddDomainEvent(new OrderStatusChangedEvent(Id, OrderNumber, oldStatus, Status));
        }

        public void Complete()
        {
            if (Status != OrderStatus.Processing)
                throw new InvalidOperationException($"Only processing orders can be completed. Current status: {Status}");

            var oldStatus = Status;
            Status = OrderStatus.Completed;
            AddDomainEvent(new OrderStatusChangedEvent(Id, OrderNumber, oldStatus, Status));
        }

        public void Cancel(string reason = "Cancelled by user")
        {
            if (Status == OrderStatus.Completed)
                throw new InvalidOperationException("Cannot cancel completed orders");

            if (Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Order is already cancelled");

            var oldStatus = Status;
            Status = OrderStatus.Cancelled;

            AddDomainEvent(new OrderCancelledEvent(Id, OrderNumber, reason));
            AddDomainEvent(new OrderStatusChangedEvent(Id, OrderNumber, oldStatus, Status));
        }

        public void UpdateNotes(string? notes)
        {
            Notes = notes;
        }

        private void RecalculateTotalAmount()
        {
            TotalAmount = _items.Sum(i => i.TotalPrice);
        }

        // Business rules
        public bool CanBeModified => Status == OrderStatus.Pending;
        public bool CanBeCancelled => Status != OrderStatus.Completed && Status != OrderStatus.Cancelled;
        public bool IsCompleted => Status == OrderStatus.Completed;
        public bool IsCancelled => Status == OrderStatus.Cancelled;
        public bool IsActive => Status != OrderStatus.Cancelled && Status != OrderStatus.Completed;
        public int ItemCount => _items.Count;
    }
}
