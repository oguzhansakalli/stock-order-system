using Inventory.Domain.Enums;
using SharedKernel.Application.Abstractions;

namespace Inventory.Application.Products.Commands.CreateProduct
{
    public record CreateProductCommand(
        string Name,
        string SKU,
        decimal Price,
        Currency Currency,
        int InitialStock,
        int LowStockThreshold,
        string? Description
    ) : ICommand<Guid>;
}
