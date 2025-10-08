using Inventory.Application.Products.DTOs;
using Inventory.Domain.Repositories;
using SharedKernel.Application.Abstractions;

namespace Inventory.Application.Products.Queries.GetProducts
{
    public class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, List<ProductDto>>
    {
        private readonly IProductRepository _repository;
        public GetProductsQueryHandler(IProductRepository repository)
        {
            _repository = repository;
        }
        public async Task<Result<List<ProductDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _repository.GetActiveProductsAsync(cancellationToken);

            var dtos = products.Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.SKU,
                p.Price.Amount,
                p.Price.Currency.ToString(),
                p.StockQuantity,
                p.LowStockThreshold,
                p.IsLowStock(),
                p.Description,
                p.IsActive,
                p.CreatedAt
            )).ToList();

            return Result<List<ProductDto>>.Success(dtos);
        }
    }
}
