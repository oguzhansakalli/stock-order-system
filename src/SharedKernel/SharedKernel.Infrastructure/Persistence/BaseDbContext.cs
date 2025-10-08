using Microsoft.EntityFrameworkCore;
using SharedKernel.Application.Abstractions;
using SharedKernel.Domain.Entities;
using System.Linq.Expressions;
using System.Reflection;

namespace SharedKernel.Infrastructure.Persistence
{
    public abstract class BaseDbContext : DbContext, IUnitOfWork
    {
        private readonly Guid _currentTenantId;
        protected BaseDbContext(DbContextOptions options, Guid currentTenantId) : base(options)
        {
            _currentTenantId = currentTenantId;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Global query filter for multi-tenancy
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var property = Expression.Property(parameter, nameof(BaseEntity.TenantId));
                    var tenantId = Expression.Constant(_currentTenantId);
                    var filter = Expression.Lambda(Expression.Equal(property, tenantId), parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
                }
            }
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Set TenantId for new entities
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity.TenantId == Guid.Empty)
                    {
                        entry.Entity.SetTenantId(_currentTenantId);
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
