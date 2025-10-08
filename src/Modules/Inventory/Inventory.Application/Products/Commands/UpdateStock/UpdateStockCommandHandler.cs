using Inventory.Domain.Repositories;
using SharedKernel.Application.Abstractions;

namespace Inventory.Application.Products.Commands.UpdateStock
{
    internal class UpdateStockCommandHandler : ICommandHandler<UpdateStockCommand>
    {
        private readonly IProductRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateStockCommandHandler(IProductRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
                return Result.Failure("Product not found");

            try
            {
                if (request.Quantity > 0)
                    product.IncreaseStock(request.Quantity);
                else if (request.Quantity < 0)
                    product.DecreaseStock(Math.Abs(request.Quantity));

                _repository.Update(product);
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
