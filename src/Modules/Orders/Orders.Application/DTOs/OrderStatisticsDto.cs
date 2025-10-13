namespace Orders.Application.DTOs
{
    public record OrderStatisticsDto
    {
        // Overall statistics
        public int TotalOrders { get; init; }
        public decimal TotalRevenue { get; init; }
        public decimal AverageOrderValue { get; init; }

        // Status breakdown
        public int PendingOrders { get; init; }
        public int ConfirmedOrders { get; init; }
        public int ProcessingOrders { get; init; }
        public int CompletedOrders { get; init; }
        public int CancelledOrders { get; init; }

        // Time-based statistics
        public int TodayOrders { get; init; }
        public decimal TodayRevenue { get; init; }

        public int WeekOrders { get; init; }
        public decimal WeekRevenue { get; init; }

        public int MonthOrders { get; init; }
        public decimal MonthRevenue { get; init; }

        // Top customers
        public List<TopCustomerDto> TopCustomers { get; init; } = new();

        // Recent orders summary
        public List<RecentOrderSummaryDto> RecentOrders { get; init; } = new();

        // Daily revenue for charts (last 30 days)
        public List<DailyRevenueDto> DailyRevenue { get; init; } = new();
    }

    public record TopCustomerDto(
        Guid CustomerId,
        string CustomerName,
        int OrderCount,
        decimal TotalSpent
    );

    public record RecentOrderSummaryDto(
        Guid OrderId,
        string OrderNumber,
        string CustomerName,
        string Status,
        decimal TotalAmount,
        DateTime CreatedAt
    );

    public record DailyRevenueDto(
        DateTime Date,
        int OrderCount,
        decimal Revenue
    );
}
