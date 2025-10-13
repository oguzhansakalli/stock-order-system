using MediatR;
using Microsoft.Extensions.Logging;
using Orders.Application.DTOs;
using Orders.Application.Services;
using Orders.Domain.Events;
using Orders.Domain.Repositories;

namespace Orders.Application.Events
{
    public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderNotificationService _notificationService;
        private readonly ILogger<OrderCreatedEventHandler> _logger;

        public OrderCreatedEventHandler(
            IOrderRepository orderRepository, 
            IOrderNotificationService notificationService, 
            ILogger<OrderCreatedEventHandler> logger)
        {
            _orderRepository = orderRepository;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Order created event received: {OrderNumber}", notification.OrderNumber);

            try
            {
                // Get full order details
                var order = await _orderRepository.GetByIdAsync(notification.OrderId, cancellationToken);

                if (order == null)
                {
                    _logger.LogWarning("Order not found: {OrderId}", notification.OrderId);
                    return;
                }

                // Map to DTO
                var orderDto = new OrderDto(
                    order.Id,
                    order.OrderNumber,
                    order.CustomerId,
                    order.CustomerName,
                    order.Status.ToString(),
                    order.TotalAmount,
                    order.Notes,
                    order.Items.Select(i => new OrderItemDto(
                        i.Id,
                        i.ProductId,
                        i.ProductName,
                        i.ProductSKU,
                        i.UnitPrice,
                        i.Quantity,
                        i.TotalPrice
                    )).ToList(),
                    order.CreatedAt
                );

                // Send real-time notification
                await _notificationService.NotifyOrderCreatedAsync(orderDto, cancellationToken);

                _logger.LogInformation("Order created notification sent successfully: {OrderNumber}", notification.OrderNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling order created event for order: {OrderNumber}", notification.OrderNumber);
            }
        }
    }
}
