using Orders.Application.DTOs;
using SharedKernel.Application.Abstractions;

namespace Orders.Application.Queries.GetOrderById
{
    public record GetOrderByIdQuery(Guid OrderId) : IQuery<OrderDto>;
}
