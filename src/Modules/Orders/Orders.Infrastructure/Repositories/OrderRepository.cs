using Microsoft.EntityFrameworkCore;
using Orders.Domain.Entities;
using Orders.Domain.Enums;
using Orders.Domain.Repositories;
using Orders.Infrastructure.DTOs;
using Orders.Infrastructure.Persistence;

namespace Orders.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrdersDbContext _context;
        public OrderRepository(OrdersDbContext context)
        {
            _context = context;
        }
        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }
        public async Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderNumber.Value == orderNumber, cancellationToken);
        }

        public async Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Order>> GetRecentOrdersAsync(int count, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .OrderByDescending(o => o.CreatedAt)
                .Take(count)
                .ToListAsync(cancellationToken);
        }
        public async Task<bool> ExistsByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .AnyAsync(o => o.OrderNumber.Value == orderNumber, cancellationToken);
        }
        // Advanced filtering with pagination
        public async Task<(List<Order> Orders, int TotalCount)> GetOrdersWithFiltersAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = _context.Orders.AsQueryable();

            // Apply filters
            if (startDate.HasValue)
                query = query.Where(o => o.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(o => o.CreatedAt <= endDate.Value);

            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);

            if (customerId.HasValue)
                query = query.Where(o => o.CustomerId == customerId.Value);

            if (!string.IsNullOrWhiteSpace(orderNumber))
                query = query.Where(o => o.OrderNumber.Value.Contains(orderNumber));

            if (!string.IsNullOrWhiteSpace(customerName))
                query = query.Where(o => o.CustomerName.Contains(customerName));

            if (minAmount.HasValue)
                query = query.Where(o => o.TotalAmount >= minAmount.Value);

            if (maxAmount.HasValue)
                query = query.Where(o => o.TotalAmount <= maxAmount.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            // Apply sorting
            query = ApplySorting(query, sortBy, sortOrder);

            // Apply pagination
            var orders = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (orders, totalCount);
        }

        public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
        {
            await _context.Orders.AddAsync(order, cancellationToken);
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }

        public void Delete(Order order)
        {
            _context.Orders.Remove(order);
        }
        // Statistics methods
        public async Task<int> GetTotalOrdersCountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Orders.CountAsync(cancellationToken);
        }
        public async Task<decimal> GetTotalRevenueAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Where(o => o.Status != OrderStatus.Cancelled)
                .SumAsync(o => o.TotalAmount, cancellationToken);
        }
        public async Task<Dictionary<OrderStatus, int>> GetOrderCountByStatusAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);
        }
        public async Task<int> GetOrdersCountByDateRangeAsync(
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default)
        {
            var startDateUtc = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            var endDateUtc = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

            return await _context.Orders
                .Where(o => o.CreatedAt >= startDateUtc && o.CreatedAt <= endDateUtc)
                .CountAsync(cancellationToken);
        }
        public async Task<decimal> GetRevenueByDateRangeAsync(
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default)
        {
            var startDateUtc = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            var endDateUtc = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

            return await _context.Orders
                .Where(o => o.CreatedAt >= startDateUtc &&
                            o.CreatedAt <= endDateUtc &&
                            o.Status != OrderStatus.Cancelled)
                .SumAsync(o => o.TotalAmount, cancellationToken);
        }
        public async Task<List<(Guid CustomerId, string CustomerName, int OrderCount, decimal TotalSpent)>> GetTopCustomersAsync(
             int count,
             CancellationToken cancellationToken = default)
        {
            var results = await _context.Orders
                .Where(o => o.Status != OrderStatus.Cancelled)
                .GroupBy(o => new { o.CustomerId, o.CustomerName })
                .Select(g => new TopCustomerQueryResult
                {
                    CustomerId = g.Key.CustomerId,
                    CustomerName = g.Key.CustomerName,
                    OrderCount = g.Count(),
                    TotalSpent = g.Sum(o => o.TotalAmount)
                })
                .OrderByDescending(x => x.TotalSpent)
                .Take(count)
                .ToListAsync(cancellationToken);

            return results
                .Select(x => (x.CustomerId, x.CustomerName, x.OrderCount, x.TotalSpent))
                .ToList();
        }

        public async Task<List<(DateTime Date, int OrderCount, decimal Revenue)>> GetDailyRevenueAsync(
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default)
        {
            var startDateUtc = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            var endDateUtc = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

            var results = await _context.Orders
                .Where(o => o.CreatedAt >= startDateUtc &&
                            o.CreatedAt <= endDateUtc &&
                            o.Status != OrderStatus.Cancelled)
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new DailyRevenueQueryResult
                {
                    Date = g.Key,
                    OrderCount = g.Count(),
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(x => x.Date)
                .ToListAsync(cancellationToken);

            return results
                .Select(x => (x.Date, x.OrderCount, x.Revenue))
                .ToList();
        }
        private static IQueryable<Order> ApplySorting(IQueryable<Order> query, string sortBy, string sortOrder)
        {
            var isDescending = sortOrder?.ToLower() == "desc";

            return sortBy?.ToLower() switch
            {
                "ordernumber" => isDescending
                    ? query.OrderByDescending(o => o.OrderNumber.Value)
                    : query.OrderBy(o => o.OrderNumber.Value),

                "totalamount" => isDescending
                    ? query.OrderByDescending(o => o.TotalAmount)
                    : query.OrderBy(o => o.TotalAmount),

                "status" => isDescending
                    ? query.OrderByDescending(o => o.Status)
                    : query.OrderBy(o => o.Status),

                "customername" => isDescending
                    ? query.OrderByDescending(o => o.CustomerName)
                    : query.OrderBy(o => o.CustomerName),

                "createdat" or _ => isDescending
                    ? query.OrderByDescending(o => o.CreatedAt)
                    : query.OrderBy(o => o.CreatedAt)
            };
        }
    }
}
