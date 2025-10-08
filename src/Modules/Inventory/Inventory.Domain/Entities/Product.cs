using Inventory.Domain.Events;
using Inventory.Domain.ValueObjects;
using SharedKernel.Domain.Entities;

namespace Inventory.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; private set; }
        public string SKU { get; private set; }
        public Money Price { get; private set; }
        public int StockQuantity { get; private set; }
        public int LowStockThreshold { get; private set; }
        public string? Description { get; private set; }
        public bool IsActive { get; private set; }

        private Product() { } // For EF Core
        public static Product Create(
            string name,
            string sku,
            Money price,
            int initialStock,
            int lowStockThreshold,
            string? description,
            Guid tenantId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be empty", nameof(name));

            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("SKU cannot be empty", nameof(sku));

            if (initialStock < 0)
                throw new ArgumentException("Initial stock cannot be negative", nameof(initialStock));

            if (lowStockThreshold < 0)
                throw new ArgumentException("Low stock threshold cannot be negative", nameof(lowStockThreshold));

            var product = new Product
            {
                Name = name,
                SKU = sku.ToUpperInvariant(),
                Price = price,
                StockQuantity = initialStock,
                LowStockThreshold = lowStockThreshold,
                Description = description,
                IsActive = true
            };

            product.SetTenantId(tenantId);
            product.AddDomainEvent(new ProductCreatedEvent(product.Id, product.Name, product.SKU));

            if (initialStock <= lowStockThreshold)
            {
                product.AddDomainEvent(new LowStockDetectedEvent(
                    product.Id,
                    product.Name,
                    product.SKU,
                    initialStock,
                    lowStockThreshold)
                );
            }

            return product;
        }
        public void UpdateStock(int quantity)
        {
            var previousStock = StockQuantity;

            if (StockQuantity + quantity < 0)
                throw new InvalidOperationException($"Stock cannot be negative. Current: {StockQuantity}, Change: {quantity}");

            StockQuantity += quantity;

            AddDomainEvent(new StockUpdatedEvent(Id, Name, previousStock, StockQuantity, quantity));

            if (StockQuantity <= LowStockThreshold)
            {
                AddDomainEvent(new LowStockDetectedEvent(Id, Name, SKU, StockQuantity, LowStockThreshold));
            }
        }
        public void DecreaseStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));

            if (StockQuantity < quantity)
                throw new InvalidOperationException($"Insufficient stock. Available: {StockQuantity}, Required: {quantity}");

            UpdateStock(-quantity);
        }

        public void IncreaseStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));

            UpdateStock(quantity);
        }

        public void UpdateDetails(string name, Money price, string? description, int lowStockThreshold)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be empty", nameof(name));

            if (lowStockThreshold < 0)
                throw new ArgumentException("Low stock threshold cannot be negative", nameof(lowStockThreshold));

            Name = name;
            Price = price;
            Description = description;
            LowStockThreshold = lowStockThreshold;

            // Check if stock is now below new threshold
            if (StockQuantity <= LowStockThreshold)
            {
                AddDomainEvent(new LowStockDetectedEvent(Id, Name, SKU, StockQuantity, LowStockThreshold));
            }
        }

        public void UpdatePrice(Money newPrice)
        {
            if (newPrice == null)
                throw new ArgumentNullException(nameof(newPrice));

            Price = newPrice;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;

        public bool IsLowStock() => StockQuantity <= LowStockThreshold;
        public bool HasStock() => StockQuantity > 0;
    }
}
