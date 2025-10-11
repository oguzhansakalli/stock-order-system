using Inventory.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Orders.Application.DTOs;
using Orders.Domain.Entities;
using Orders.Domain.Repositories;
using SharedKernel.Application.Abstractions;

namespace Orders.Application.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _ordersUnitOfWork;
        private readonly IUnitOfWork _inventoryUnitOfWork;
        private readonly ITenantProvider _tenantProvider;
        public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        [FromKeyedServices("Orders")] IUnitOfWork ordersUnitOfWork,
        [FromKeyedServices("Inventory")] IUnitOfWork inventoryUnitOfWork,
        ITenantProvider tenantProvider)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _ordersUnitOfWork = ordersUnitOfWork;
            _inventoryUnitOfWork = inventoryUnitOfWork;
            _tenantProvider = tenantProvider;
        }
        public async Task<Result<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Create order
                var order = Order.Create(
                    request.CustomerId,
                    request.CustomerName,
                    request.Notes,
                    _tenantProvider.GetCurrentTenantId()
                );

                // Add items and decrease stock
                foreach (var itemRequest in request.Items)
                {
                    var product = await _productRepository.GetByIdAsync(itemRequest.ProductId, cancellationToken);
                    if (product == null)
                        return Result<OrderDto>.Failure($"Product with ID {itemRequest.ProductId} not found");

                    if (!product.IsActive)
                        return Result<OrderDto>.Failure($"Product '{product.Name}' is not active");

                    if (product.StockQuantity < itemRequest.Quantity)
                        return Result<OrderDto>.Failure(
                            $"Insufficient stock for '{product.Name}'. Available: {product.StockQuantity}, Required: {itemRequest.Quantity}");

                    // Create order item
                    var orderItem = OrderItem.Create(
                        product.Id,
                        product.Name,
                        product.SKU,
                        product.Price.Amount,
                        itemRequest.Quantity
                    );

                    order.AddItem(orderItem);

                    // Decrease stock
                    product.DecreaseStock(itemRequest.Quantity);
                    _productRepository.Update(product);
                }

                // Confirm order
                order.Confirm();

                // Save to database
                await _orderRepository.AddAsync(order, cancellationToken);
                await _ordersUnitOfWork.SaveChangesAsync(cancellationToken);
                await _inventoryUnitOfWork.SaveChangesAsync(cancellationToken);

                // Return DTO
                var orderDto = MapToDto(order);
                return Result<OrderDto>.Success(orderDto);
            }
            catch (Exception ex)
            {
                return Result<OrderDto>.Failure(ex.Message);
            }
        }
        private static OrderDto MapToDto(Order order)
        {
            return new OrderDto(
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
        }
    }
}
