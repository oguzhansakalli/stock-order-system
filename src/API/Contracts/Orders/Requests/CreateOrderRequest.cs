namespace API.Contracts.Orders.Requests
{
    public record CreateOrderRequest(
        string CustomerName,
        string? Notes,
        List<OrderItemRequest> Items
    );

    public record OrderItemRequest(
        Guid ProductId,
        int Quantity
    );
}
