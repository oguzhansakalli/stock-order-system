using Inventory.Application.Products.DTOs;
using SharedKernel.Application.Abstractions;

namespace Inventory.Application.Products.Queries.GetLowStockProducts
{
    public record GetLowStockProductsQuery : IQuery<List<ProductDto>>;
}
