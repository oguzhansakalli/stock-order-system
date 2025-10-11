using Orders.Domain.Entities;
using Orders.Domain.Enums;

namespace Orders.Domain.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
        Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<List<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
        Task<List<Order>> GetRecentOrdersAsync(int count, CancellationToken cancellationToken = default);
        Task<bool> ExistsByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
        Task AddAsync(Order order, CancellationToken cancellationToken = default);
        void Update(Order order);
        void Delete(Order order);
    }
}
