using Orders.Application.DTOs;
using Orders.Domain.Enums;

namespace Orders.Application.Services
{
    public interface IOrderNotificationService
    {
        Task NotifyOrderCreatedAsync(OrderDto order, CancellationToken cancellationToken = default);
        Task NotifyOrderStatusChangedAsync(Guid orderId, string orderNumber, OrderStatus oldStatus, OrderStatus newStatus, CancellationToken cancellationToken = default);
        Task NotifyOrderCancelledAsync(Guid orderId, string orderNumber, string reason, CancellationToken cancellationToken = default);
    }
}
