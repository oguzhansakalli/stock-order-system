using Inventory.Application.Products.DTOs;
using SharedKernel.Application.Abstractions;

namespace Inventory.Application.Products.Queries.GetProducts
{
    public record GetProductsQuery : IQuery<List<ProductDto>>;
}
