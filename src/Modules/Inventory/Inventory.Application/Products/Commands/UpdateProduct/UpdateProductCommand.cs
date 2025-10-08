using Inventory.Domain.Enums;
using SharedKernel.Application.Abstractions;

namespace Inventory.Application.Products.Commands.UpdateProduct
{
    public record UpdateProductCommand(
        Guid ProductId,
        string Name,
        decimal Price,
        Currency Currency,
        string? Description,
        int LowStockThreshold
    ) : ICommand;
}
