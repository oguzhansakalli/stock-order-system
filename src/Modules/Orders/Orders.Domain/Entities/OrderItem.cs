namespace Orders.Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; }
        public string ProductSKU { get; private set; }
        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }
        public decimal TotalPrice { get; private set; }
        private OrderItem() { } // For EF Core
        public static OrderItem Create(
            Guid productId,
            string productName,
            string productSKU,
            decimal unitPrice,
            int quantity)
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException("Product name cannot be empty", nameof(productName));

            if (string.IsNullOrWhiteSpace(productSKU))
                throw new ArgumentException("Product SKU cannot be empty", nameof(productSKU));

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));

            if (unitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                ProductName = productName,
                ProductSKU = productSKU,
                UnitPrice = unitPrice,
                Quantity = quantity,
                TotalPrice = unitPrice * quantity
            };

            return orderItem;
        }
        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(newQuantity));

            Quantity = newQuantity;
            RecalculateTotalPrice();
        }

        public void UpdateUnitPrice(decimal newUnitPrice)
        {
            if (newUnitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative", nameof(newUnitPrice));

            UnitPrice = newUnitPrice;
            RecalculateTotalPrice();
        }

        private void RecalculateTotalPrice()
        {
            TotalPrice = UnitPrice * Quantity;
        }
    }
}
