namespace Orders.Infrastructure.DTOs
{
    internal class DailyRevenueQueryResult
    {
        public DateTime Date { get; set; }
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
    }
}
