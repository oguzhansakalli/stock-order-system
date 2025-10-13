namespace Orders.Infrastructure.DTOs
{
    internal class TopCustomerQueryResult
    {
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int OrderCount { get; set; }
        public decimal TotalSpent { get; set; }
    }
}
