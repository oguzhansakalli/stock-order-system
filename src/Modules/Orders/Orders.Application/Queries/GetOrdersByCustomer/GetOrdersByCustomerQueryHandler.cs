using Orders.Application.DTOs;
using Orders.Domain.Repositories;
using SharedKernel.Application.Abstractions;

namespace Orders.Application.Queries.GetOrdersByCustomer
{
    public class GetOrdersByCustomerQueryHandler : IQueryHandler<GetOrdersByCustomerQuery, List<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrdersByCustomerQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Result<List<OrderDto>>> Handle(GetOrdersByCustomerQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);

            var orderDtos = orders.Select(o => new OrderDto(
                o.Id,
                o.OrderNumber,
                o.CustomerId,
                o.CustomerName,
                o.Status.ToString(),
                o.TotalAmount,
                o.Notes,
                o.Items.Select(i => new OrderItemDto(
                    i.Id,
                    i.ProductId,
                    i.ProductName,
                    i.ProductSKU,
                    i.UnitPrice,
                    i.Quantity,
                    i.TotalPrice
                )).ToList(),
                o.CreatedAt
            )).ToList();

            return Result<List<OrderDto>>.Success(orderDtos);
        }
    }
}
