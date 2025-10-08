using Inventory.Application.Products.DTOs;
using SharedKernel.Application.Abstractions;

namespace Inventory.Application.Products.Queries.GetProductById
{
    public record GetProductByIdQuery(Guid ProductId) : IQuery<ProductDto>;
}
