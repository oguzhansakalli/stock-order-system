using Orders.Application.DTOs;
using Orders.Domain.Enums;
using Orders.Domain.Repositories;
using SharedKernel.Application.Abstractions;

namespace Orders.Application.Queries.GetOrderStatistics
{
    public class GetOrderStatisticsQueryHandler : IQueryHandler<GetOrderStatisticsQuery, OrderStatisticsDto>
    {
        private readonly IOrderRepository _orderRepository;
        public GetOrderStatisticsQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task<Result<OrderStatisticsDto>> Handle(GetOrderStatisticsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var now = DateTime.UtcNow;
                var today = now.Date;
                var weekStart = today.AddDays(-(int)today.DayOfWeek);
                var monthStart = new DateTime(now.Year, now.Month, 1);

                // Overall statistics
                var totalOrders = await _orderRepository.GetTotalOrdersCountAsync(cancellationToken);
                var totalRevenue = await _orderRepository.GetTotalRevenueAsync(cancellationToken);
                var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

                // Status breakdown
                var ordersByStatus = await _orderRepository.GetOrderCountByStatusAsync(cancellationToken);

                var pendingOrders = ordersByStatus.GetValueOrDefault(OrderStatus.Pending, 0);
                var confirmedOrders = ordersByStatus.GetValueOrDefault(OrderStatus.Confirmed, 0);
                var processingOrders = ordersByStatus.GetValueOrDefault(OrderStatus.Processing, 0);
                var completedOrders = ordersByStatus.GetValueOrDefault(OrderStatus.Completed, 0);
                var cancelledOrders = ordersByStatus.GetValueOrDefault(OrderStatus.Cancelled, 0);

                // Today's statistics
                var todayOrders = await _orderRepository.GetOrdersCountByDateRangeAsync(
                    today,
                    today.AddDays(1).AddTicks(-1),
                    cancellationToken);

                var todayRevenue = await _orderRepository.GetRevenueByDateRangeAsync(
                    today,
                    today.AddDays(1).AddTicks(-1),
                    cancellationToken);

                // This week's statistics
                var weekOrders = await _orderRepository.GetOrdersCountByDateRangeAsync(
                    weekStart,
                    now,
                    cancellationToken);

                var weekRevenue = await _orderRepository.GetRevenueByDateRangeAsync(
                    weekStart,
                    now,
                    cancellationToken);

                // This month's statistics
                var monthOrders = await _orderRepository.GetOrdersCountByDateRangeAsync(
                    monthStart,
                    now,
                    cancellationToken);

                var monthRevenue = await _orderRepository.GetRevenueByDateRangeAsync(
                    monthStart,
                    now,
                    cancellationToken);

                // Top customers (top 10)
                var topCustomersData = await _orderRepository.GetTopCustomersAsync(10, cancellationToken);
                var topCustomers = topCustomersData
                    .Select(x => new TopCustomerDto(x.Item1, x.Item2, x.Item3, x.Item4))
                    .ToList();

                // Recent orders (last 10)
                var recentOrders = await _orderRepository.GetRecentOrdersAsync(10, cancellationToken);
                var recentOrdersSummary = recentOrders
                    .Select(o => new RecentOrderSummaryDto(
                        o.Id,
                        o.OrderNumber,
                        o.CustomerName,
                        o.Status.ToString(),
                        o.TotalAmount,
                        o.CreatedAt
                    ))
                    .ToList();

                // Daily revenue for last 30 days
                var thirtyDaysAgo = today.AddDays(-30);
                var dailyRevenueData = await _orderRepository.GetDailyRevenueAsync(
                    thirtyDaysAgo,
                    now,
                    cancellationToken);

                var dailyRevenue = dailyRevenueData
                    .Select(x => new DailyRevenueDto(x.Item1, x.Item2, x.Item3))
                    .ToList();

                var statistics = new OrderStatisticsDto
                {
                    TotalOrders = totalOrders,
                    TotalRevenue = totalRevenue,
                    AverageOrderValue = averageOrderValue,

                    PendingOrders = pendingOrders,
                    ConfirmedOrders = confirmedOrders,
                    ProcessingOrders = processingOrders,
                    CompletedOrders = completedOrders,
                    CancelledOrders = cancelledOrders,

                    TodayOrders = todayOrders,
                    TodayRevenue = todayRevenue,

                    WeekOrders = weekOrders,
                    WeekRevenue = weekRevenue,

                    MonthOrders = monthOrders,
                    MonthRevenue = monthRevenue,

                    TopCustomers = topCustomers,
                    RecentOrders = recentOrdersSummary,
                    DailyRevenue = dailyRevenue
                };

                return Result<OrderStatisticsDto>.Success(statistics);
            }
            catch (Exception ex)
            {
                return Result<OrderStatisticsDto>.Failure($"Error fetching order statistics: {ex.Message}");
            }
        }
    }
}
