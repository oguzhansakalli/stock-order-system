using Inventory.Domain.Enums;

namespace API.Contracts.Products.Requests
{
    public record CreateProductRequest(
        string Name,
        string SKU,
        decimal Price,
        Currency Currency,
        int InitialStock,
        int LowStockThreshold,
        string? Description
    );
}
