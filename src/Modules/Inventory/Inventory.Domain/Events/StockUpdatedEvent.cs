using SharedKernel.Domain.Events;

namespace Inventory.Domain.Events
{
    public sealed class StockUpdatedEvent :DomainEvent
    {
        public Guid ProductId { get; }
        public string ProductName { get; }
        public int PreviousStock { get; }
        public int NewStock { get; }
        public int ChangeAmount { get; }
        public StockUpdatedEvent(
            Guid productId,
            string productName,
            int previousStock,
            int newStock,
            int changeAmount)
        {
            ProductId = productId;
            ProductName = productName;
            PreviousStock = previousStock;
            NewStock = newStock;
            ChangeAmount = changeAmount;
        }
    }
}
