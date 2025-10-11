using Orders.Application.DTOs;
using SharedKernel.Application.Abstractions;

namespace Orders.Application.Commands.CreateOrder
{
    public record CreateOrderCommand(
        Guid CustomerId,
        string CustomerName,
        string? Notes,
        List<OrderItemRequest> Items
    ) : ICommand<OrderDto>;

    public record OrderItemRequest(
        Guid ProductId,
        int Quantity
    );
}
