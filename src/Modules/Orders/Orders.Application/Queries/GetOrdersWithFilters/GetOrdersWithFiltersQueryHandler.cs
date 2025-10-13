using Orders.Application.DTOs;
using Orders.Domain.Repositories;
using SharedKernel.Application.Abstractions;
using SharedKernel.Application.DTOs;

namespace Orders.Application.Queries.GetOrdersWithFilters
{
    public class GetOrdersWithFiltersQueryHandler : IQueryHandler<GetOrdersWithFiltersQuery, PagedResult<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        public GetOrdersWithFiltersQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task<Result<PagedResult<OrderDto>>> Handle(GetOrdersWithFiltersQuery request, CancellationToken cancellationToken)
        {
            var filters = request.Filters;

            // Get filtered and paginated orders
            var (orders, totalCount) = await _orderRepository.GetOrdersWithFiltersAsync(
                filters.StartDate,
                filters.EndDate,
                filters.Status,
                filters.CustomerId,
                filters.OrderNumber,
                filters.CustomerName,
                filters.MinAmount,
                filters.MaxAmount,
                filters.PageNumber,
                filters.PageSize,
                filters.SortBy ?? "CreatedAt",
                filters.SortOrder ?? "desc",
                cancellationToken
            );

            // Map to DTOs
            var orderDtos = orders.Select(o => new OrderDto(
                o.Id,
                o.OrderNumber,
                o.CustomerId,
                o.CustomerName,
                o.Status.ToString(),
                o.TotalAmount,
                o.Notes,
                o.Items.Select(i => new OrderItemDto(
                    i.Id,
                    i.ProductId,
                    i.ProductName,
                    i.ProductSKU,
                    i.UnitPrice,
                    i.Quantity,
                    i.TotalPrice
                )).ToList(),
                o.CreatedAt
            )).ToList();

            var pagedResult = new PagedResult<OrderDto>(
                orderDtos,
                totalCount,
                filters.PageNumber,
                filters.PageSize
            );

            return Result<PagedResult<OrderDto>>.Success(pagedResult);
        }
    }
}
