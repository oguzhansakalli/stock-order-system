using Microsoft.EntityFrameworkCore;
using SharedKernel.Application.Abstractions;
using SharedKernel.Domain.Entities;
using SharedKernel.Domain.Events;
using System.Linq.Expressions;
using System.Reflection;

namespace SharedKernel.Infrastructure.Persistence
{
    public abstract class BaseDbContext : DbContext, IUnitOfWork
    {
        private readonly Guid _currentTenantId;
        private readonly IDomainEventDispatcher? _domainEventDispatcher;
        protected BaseDbContext(DbContextOptions options, Guid currentTenantId, IDomainEventDispatcher? domainEventDispatcher = null) : base(options)
        {
            _currentTenantId = currentTenantId;
            _domainEventDispatcher = domainEventDispatcher;
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
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
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

            // Collect domain events before saving
            var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
                 .Where(e => e.Entity.DomainEvents.Any())
                 .Select(e => e.Entity)
                 .ToList();

            var domainEvents = entitiesWithEvents
                .SelectMany(e => e.DomainEvents)
                .ToList();

            // Clear domain events from entities
            entitiesWithEvents.ForEach(e => e.ClearDomainEvents());

            // Save changes to database
            var result = await base.SaveChangesAsync(cancellationToken);

            // Dispatch domain events after successful save (if dispatcher is available)
            if (_domainEventDispatcher != null && domainEvents.Any())
            {
                await _domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);
            }

            return result;
        }
    }
}
