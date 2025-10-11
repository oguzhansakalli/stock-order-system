using SharedKernel.Application.Abstractions;

namespace Orders.Application.Commands.CancelOrder
{
    public record CancelOrderCommand(Guid OrderId, string? Reason) : ICommand;
}
