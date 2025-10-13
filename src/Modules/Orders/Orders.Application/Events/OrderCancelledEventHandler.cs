using MediatR;
using Microsoft.Extensions.Logging;
using Orders.Application.Services;
using Orders.Domain.Events;

namespace Orders.Application.Events
{
    public class OrderCancelledEventHandler : INotificationHandler<OrderCancelledEvent>
    {
        private readonly IOrderNotificationService _notificationService;
        private readonly ILogger<OrderCancelledEventHandler> _logger;

        public OrderCancelledEventHandler(
            IOrderNotificationService notificationService,
            ILogger<OrderCancelledEventHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }
        public async Task Handle(OrderCancelledEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Order cancelled event received: {OrderNumber}, Reason: {Reason}",
                notification.OrderNumber,
                notification.Reason
            );

            try
            {
                // Send real-time notification
                await _notificationService.NotifyOrderCancelledAsync(
                    notification.OrderId,
                    notification.OrderNumber,
                    notification.Reason ?? "No reason provided",
                    cancellationToken
                );

                _logger.LogInformation(
                    "Order cancelled notification sent successfully: {OrderNumber}",
                    notification.OrderNumber
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error handling order cancelled event for order: {OrderNumber}",
                    notification.OrderNumber
                );
            }

        }
    }
}
