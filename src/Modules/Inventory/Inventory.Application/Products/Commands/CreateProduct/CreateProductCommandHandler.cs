using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.ValueObjects;
using SharedKernel.Application.Abstractions;

namespace Inventory.Application.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, Guid>
    {
        private readonly IProductRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantProvider _tenantProvider;

        public CreateProductCommandHandler(
            IProductRepository repository,
            IUnitOfWork unitOfWork,
            ITenantProvider tenantProvider)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _tenantProvider = tenantProvider;
        }

        public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // Check if SKU already exists
            var skuExists = await _repository.ExistsBySkuAsync(request.SKU, cancellationToken);
            if (skuExists)
                return Result<Guid>.Failure($"A product with SKU '{request.SKU}' already exists");

            try
            {
                var product = Product.Create(
                    request.Name,
                    request.SKU,
                    new Money(request.Price, request.Currency),
                    request.InitialStock,
                    request.LowStockThreshold,
                    request.Description,
                    _tenantProvider.GetCurrentTenantId()
                );

                await _repository.AddAsync(product, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(product.Id);
            }
            catch (ArgumentException ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }
        }
    }
}
