using Inventory.Domain.Repositories;
using Inventory.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Application.Abstractions;

namespace Inventory.Application.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand>
    {
        private readonly IProductRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateProductCommandHandler(IProductRepository repository, [FromKeyedServices("Inventory")] IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
                return Result.Failure("Product not found");

            try
            {
                product.UpdateDetails(
                    request.Name,
                    new Money(request.Price, request.Currency),
                    request.Description,
                    request.LowStockThreshold
                );

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
