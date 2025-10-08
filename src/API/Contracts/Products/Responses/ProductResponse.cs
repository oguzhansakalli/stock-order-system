namespace API.Contracts.Products.Responses
{
    public record ProductResponse(
        Guid Id,
        string Name,
        string SKU,
        decimal Price,
        string Currency,
        int StockQuantity,
        int LowStockThreshold,
        bool IsLowStock,
        string? Description,
        bool IsActive,
        DateTime CreatedAt
    );
}
