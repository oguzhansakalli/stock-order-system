using Microsoft.EntityFrameworkCore;
using Orders.Domain.Entities;
using SharedKernel.Application.Abstractions;
using SharedKernel.Infrastructure.Persistence;

namespace Orders.Infrastructure.Persistence
{
    public class OrdersDbContext : BaseDbContext
    {
        private readonly ITenantProvider _tenantProvider;
        public DbSet<Order> Orders => Set<Order>();
        // Constructor for runtime
        public OrdersDbContext(
            DbContextOptions<OrdersDbContext> options,
            ITenantProvider tenantProvider,
            IDomainEventDispatcher domainEventDispatcher)
            : base(options, tenantProvider.GetCurrentTenantId(), domainEventDispatcher)
        {
            _tenantProvider = tenantProvider;
        }

        // Constructor for design-time (migrations)
        public OrdersDbContext(DbContextOptions<OrdersDbContext> options)
            : base(options, Guid.Empty, null)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("orders");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrdersDbContext).Assembly);
        }
    }
}
