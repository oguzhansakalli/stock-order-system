using Microsoft.Extensions.DependencyInjection;
using Orders.Domain.Repositories;
using SharedKernel.Application.Abstractions;

namespace Orders.Application.Commands.CancelOrder
{
    public class CancelOrderCommandHandler : ICommandHandler<CancelOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelOrderCommandHandler(
            IOrderRepository orderRepository,
            [FromKeyedServices("Orders")] IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
                return Result.Failure("Order not found");

            if (!order.CanBeCancelled)
                return Result.Failure($"Order with status '{order.Status}' cannot be cancelled");

            try
            {
                order.Cancel(request.Reason ?? "Cancelled by user");
                _orderRepository.Update(order);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
