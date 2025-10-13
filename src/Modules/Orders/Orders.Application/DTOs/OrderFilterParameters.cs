using Orders.Domain.Enums;

namespace Orders.Application.DTOs
{
    public class OrderFilterParameters
    {
        public DateTime? StartDate { get; init; }
        public DateTime? EndDate { get; init; }
        public OrderStatus? Status { get; init; }
        public Guid? CustomerId { get; init; }
        public string? OrderNumber { get; init; }
        public string? CustomerName { get; init; }
        public decimal? MinAmount { get; init; }
        public decimal? MaxAmount { get; init; }

        // Pagination
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;

        // Sorting
        public string? SortBy { get; init; } = "CreatedAt"; // CreatedAt, OrderNumber, TotalAmount, Status
        public string? SortOrder { get; init; } = "desc"; // asc, desc
    }
}
