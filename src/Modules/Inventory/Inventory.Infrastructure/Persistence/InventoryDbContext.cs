using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Application.Abstractions;
using SharedKernel.Infrastructure.Persistence;

namespace Inventory.Infrastructure.Persistence
{
    public class InventoryDbContext : BaseDbContext
    {
        private readonly ITenantProvider? _tenantProvider;
        public DbSet<Product> Products { get; set; }
        // Constructor for runtime
        public InventoryDbContext(
            DbContextOptions<InventoryDbContext> options,
            ITenantProvider tenantProvider)
            : base(options, tenantProvider.GetCurrentTenantId())
        {
            _tenantProvider = tenantProvider;
        }
        // Constructor for design-time (migrations)
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
            : base(options, Guid.Empty)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("inventory");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryDbContext).Assembly);
        }
    }
}
