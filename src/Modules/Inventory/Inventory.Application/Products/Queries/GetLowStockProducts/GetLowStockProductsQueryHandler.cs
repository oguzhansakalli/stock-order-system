using Inventory.Application.Products.DTOs;
using Inventory.Domain.Repositories;
using SharedKernel.Application.Abstractions;

namespace Inventory.Application.Products.Queries.GetLowStockProducts
{
    public class GetLowStockProductsQueryHandler : IQueryHandler<GetLowStockProductsQuery, List<ProductDto>>
    {
        private readonly IProductRepository _repository;
        public GetLowStockProductsQueryHandler(IProductRepository repository)
        {
            _repository = repository;
        }
        public async Task<Result<List<ProductDto>>> Handle(GetLowStockProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _repository.GetLowStockProductsAsync(cancellationToken);

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
