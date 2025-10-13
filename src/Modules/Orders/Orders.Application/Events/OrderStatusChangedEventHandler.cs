using MediatR;
using Microsoft.Extensions.Logging;
using Orders.Application.Services;
using Orders.Domain.Events;

namespace Orders.Application.Events
{
    public class OrderStatusChangedEventHandler : INotificationHandler<OrderStatusChangedEvent>
    {
        private readonly IOrderNotificationService _notificationService;
        private readonly ILogger<OrderStatusChangedEventHandler> _logger;

        public OrderStatusChangedEventHandler(
            IOrderNotificationService notificationService, 
            ILogger<OrderStatusChangedEventHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Handle(OrderStatusChangedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Order status changed event received: {OrderNumber} from {OldStatus} to {NewStatus}",
                notification.OrderNumber,
                notification.OldStatus,
                notification.NewStatus
            );

            try
            {
                // Send real-time notification
                await _notificationService.NotifyOrderStatusChangedAsync(
                    notification.OrderId,
                    notification.OrderNumber,
                    notification.OldStatus,
                    notification.NewStatus,
                    cancellationToken
                );

                _logger.LogInformation(
                    "Order status changed notification sent successfully: {OrderNumber}",
                    notification.OrderNumber
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error handling order status changed event for order: {OrderNumber}",
                    notification.OrderNumber
                );
            }
        }
    }
}
