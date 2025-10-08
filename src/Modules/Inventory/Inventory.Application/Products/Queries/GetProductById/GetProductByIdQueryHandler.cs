using Inventory.Application.Products.DTOs;
using Inventory.Domain.Repositories;
using SharedKernel.Application.Abstractions;

namespace Inventory.Application.Products.Queries.GetProductById
{
    public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IProductRepository _repository;
        public GetProductByIdQueryHandler(IProductRepository repository)
        {
            _repository = repository;
        }
        public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(request.ProductId, cancellationToken);

            if (product == null)
                return Result<ProductDto>.Failure("Product not found");

            var dto = new ProductDto(
                product.Id,
                product.Name,
                product.SKU,
                product.Price.Amount,
                product.Price.Currency.ToString(),
                product.StockQuantity,
                product.LowStockThreshold,
                product.IsLowStock(),
                product.Description,
                product.IsActive,
                product.CreatedAt
            );

            return Result<ProductDto>.Success(dto);
        }
    }
}
