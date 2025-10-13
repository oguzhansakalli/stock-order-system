using Orders.Application.DTOs;
using SharedKernel.Application.Abstractions;
using SharedKernel.Application.DTOs;

namespace Orders.Application.Queries.GetOrdersWithFilters
{
    public record GetOrdersWithFiltersQuery(OrderFilterParameters Filters)
       : IQuery<PagedResult<OrderDto>>;
}
