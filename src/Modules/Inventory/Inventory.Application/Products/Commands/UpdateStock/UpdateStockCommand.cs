using SharedKernel.Application.Abstractions;

namespace Inventory.Application.Products.Commands.UpdateStock
{
    public record UpdateStockCommand(Guid ProductId, int Quantity) : ICommand;
}
