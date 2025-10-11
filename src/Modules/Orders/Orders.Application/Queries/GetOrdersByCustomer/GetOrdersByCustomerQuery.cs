using Orders.Application.DTOs;
using SharedKernel.Application.Abstractions;

namespace Orders.Application.Queries.GetOrdersByCustomer
{
    public record GetOrdersByCustomerQuery(Guid CustomerId) : IQuery<List<OrderDto>>;
}
