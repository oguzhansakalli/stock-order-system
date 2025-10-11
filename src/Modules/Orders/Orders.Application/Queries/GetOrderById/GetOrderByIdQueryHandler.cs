using Orders.Application.DTOs;
using Orders.Domain.Repositories;
using SharedKernel.Application.Abstractions;

namespace Orders.Application.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

            if (order == null)
                return Result<OrderDto>.Failure("Order not found");

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

            return Result<OrderDto>.Success(orderDto);
        }
    }
}
