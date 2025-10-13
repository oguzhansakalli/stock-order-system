using API.Hubs;
using Microsoft.AspNetCore.SignalR;
using Orders.Application.DTOs;
using Orders.Application.Services;
using Orders.Domain.Enums;

namespace API.Services
{
    public class OrderNotificationService : IOrderNotificationService
    {
        private readonly IHubContext<OrderHub> _hubContext;
        private readonly ILogger<OrderNotificationService> _logger;
        public OrderNotificationService(
            IHubContext<OrderHub> hubContext, 
            ILogger<OrderNotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }
        public async Task NotifyOrderCreatedAsync(OrderDto order, CancellationToken cancellationToken = default)
        {
            try
            {
                await _hubContext.Clients.Group("orders").SendAsync(
                    "OrderCreated",
                    new
                    {
                        orderId = order.Id,
                        orderNumber = order.OrderNumber,
                        customerName = order.CustomerName,
                        totalAmount = order.TotalAmount,
                        itemCount = order.Items.Count,
                        createdAt = order.CreatedAt
                    },
                    cancellationToken);

                // Also notify specific customer
                await _hubContext.Clients.Group($"user_{order.CustomerId}").SendAsync(
                    "OrderCreated",
                    new
                    {
                        orderId = order.Id,
                        orderNumber = order.OrderNumber,
                        totalAmount = order.TotalAmount,
                        message = $"Your order {order.OrderNumber} has been created successfully!"
                    },
                    cancellationToken);

                _logger.LogInformation("Order created notification sent: {OrderNumber}", order.OrderNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending order created notification for order {OrderNumber}", order.OrderNumber);
            }
        }
        public async Task NotifyOrderStatusChangedAsync(
            Guid orderId,
            string orderNumber,
            OrderStatus oldStatus,
            OrderStatus newStatus,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _hubContext.Clients.Group("orders").SendAsync(
                    "OrderStatusChanged",
                    new
                    {
                        orderId,
                        orderNumber,
                        oldStatus = oldStatus.ToString(),
                        newStatus = newStatus.ToString(),
                        changedAt = DateTime.UtcNow
                    },
                    cancellationToken);

                _logger.LogInformation("Order status changed notification sent: {OrderNumber} from {OldStatus} to {NewStatus}",
                    orderNumber, oldStatus, newStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending order status changed notification for order {OrderNumber}", orderNumber);
            }
        }

        public async Task NotifyOrderCancelledAsync(
            Guid orderId,
            string orderNumber,
            string reason,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _hubContext.Clients.Group("orders").SendAsync(
                    "OrderCancelled",
                    new
                    {
                        orderId,
                        orderNumber,
                        reason,
                        cancelledAt = DateTime.UtcNow
                    },
                    cancellationToken);

                _logger.LogInformation("Order cancelled notification sent: {OrderNumber}, Reason: {Reason}",
                    orderNumber, reason);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending order cancelled notification for order {OrderNumber}", orderNumber);
            }
        }
    }
}
