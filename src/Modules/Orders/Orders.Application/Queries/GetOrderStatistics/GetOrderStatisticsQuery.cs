using Orders.Application.DTOs;
using SharedKernel.Application.Abstractions;

namespace Orders.Application.Queries.GetOrderStatistics
{
    public record GetOrderStatisticsQuery(
        DateTime? StartDate = null,
        DateTime? EndDate = null
    ) : IQuery<OrderStatisticsDto>;
}
