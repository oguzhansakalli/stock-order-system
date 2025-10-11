namespace Orders.Application.DTOs
{
    public record OrderDto(
        Guid Id,
        string OrderNumber,
        Guid CustomerId,
        string CustomerName,
        string Status,
        decimal TotalAmount,
        string? Notes,
        List<OrderItemDto> Items,
        DateTime CreatedAt
    );

    public record OrderItemDto(
        Guid Id,
        Guid ProductId,
        string ProductName,
        string ProductSKU,
        decimal UnitPrice,
        int Quantity,
        decimal TotalPrice
    );
}
