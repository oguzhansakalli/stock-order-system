using Orders.Domain.Entities;
using Orders.Domain.Enums;

namespace Orders.Domain.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
        Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<List<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
        Task<List<Order>> GetRecentOrdersAsync(int count, CancellationToken cancellationToken = default);
        Task<bool> ExistsByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
        // Filtering and pagination
        Task<(List<Order> Orders, int TotalCount)> GetOrdersWithFiltersAsync(
            DateTime? startDate,
            DateTime? endDate,
            OrderStatus? status,
            Guid? customerId,
            string? orderNumber,
            string? customerName,
            decimal? minAmount,
            decimal? maxAmount,
            int pageNumber,
            int pageSize,
            string sortBy,
            string sortOrder,
            CancellationToken cancellationToken = default);
        Task AddAsync(Order order, CancellationToken cancellationToken = default);
        void Update(Order order);
        void Delete(Order order);

        // Statistics methods
        Task<int> GetTotalOrdersCountAsync(CancellationToken cancellationToken = default);
        Task<decimal> GetTotalRevenueAsync(CancellationToken cancellationToken = default);
        Task<Dictionary<OrderStatus, int>> GetOrderCountByStatusAsync(CancellationToken cancellationToken = default);

        Task<int> GetOrdersCountByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<decimal> GetRevenueByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

        Task<List<(Guid CustomerId, string CustomerName, int OrderCount, decimal TotalSpent)>> GetTopCustomersAsync(
            int count,
            CancellationToken cancellationToken = default);

        Task<List<(DateTime Date, int OrderCount, decimal Revenue)>> GetDailyRevenueAsync(
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default);
    }
}
