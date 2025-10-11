using Microsoft.EntityFrameworkCore;
using Orders.Domain.Entities;
using Orders.Domain.Enums;
using Orders.Domain.Repositories;
using Orders.Infrastructure.Persistence;

namespace Orders.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrdersDbContext _context;
        public OrderRepository(OrdersDbContext context)
        {
            _context = context;
        }
        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }
        public async Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderNumber.Value == orderNumber, cancellationToken);
        }

        public async Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Order>> GetRecentOrdersAsync(int count, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .OrderByDescending(o => o.CreatedAt)
                .Take(count)
                .ToListAsync(cancellationToken);
        }
        public async Task<bool> ExistsByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .AnyAsync(o => o.OrderNumber.Value == orderNumber, cancellationToken);
        }

        public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
        {
            await _context.Orders.AddAsync(order, cancellationToken);
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }

        public void Delete(Order order)
        {
            _context.Orders.Remove(order);
        }
    }
}
