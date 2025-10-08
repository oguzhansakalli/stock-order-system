using Inventory.Domain.Enums;

namespace API.Contracts.Products.Requests
{
    public record UpdateProductRequest(
        string Name,
        decimal Price,
        Currency Currency,
        string? Description,
        int LowStockThreshold
    );
}
