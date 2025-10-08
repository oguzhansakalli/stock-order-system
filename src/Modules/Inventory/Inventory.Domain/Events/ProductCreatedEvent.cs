using SharedKernel.Domain.Events;

namespace Inventory.Domain.Events
{
    public sealed class ProductCreatedEvent : DomainEvent
    {
        public Guid ProductId { get; }
        public string ProductName { get; }
        public string SKU { get; }

        public ProductCreatedEvent(Guid productId, string productName, string sku)
        {
            ProductId = productId;
            ProductName = productName;
            SKU = sku;
        }
    }
}
