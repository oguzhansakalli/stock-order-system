using SharedKernel.Domain.Events;

namespace Inventory.Domain.Events
{
    public sealed class LowStockDetectedEvent : DomainEvent
    {
        public Guid ProductId { get; }
        public string ProductName { get; }
        public string SKU { get; }
        public int CurrentStock { get; }
        public int Threshold { get; }

        public LowStockDetectedEvent(
            Guid productId,
            string productName,
            string sku,
            int currentStock,
            int threshold)
        {
            ProductId = productId;
            ProductName = productName;
            SKU = sku;
            CurrentStock = currentStock;
            Threshold = threshold;
        }
    }
}
