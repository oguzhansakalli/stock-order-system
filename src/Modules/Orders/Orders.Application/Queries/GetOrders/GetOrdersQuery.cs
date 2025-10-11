using Orders.Application.DTOs;
using SharedKernel.Application.Abstractions;

namespace Orders.Application.Queries.GetOrders
{
    public record GetOrdersQuery : IQuery<List<OrderDto>>;
}
