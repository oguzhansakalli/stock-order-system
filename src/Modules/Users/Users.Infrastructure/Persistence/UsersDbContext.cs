using Microsoft.EntityFrameworkCore;
using SharedKernel.Application.Abstractions;
using SharedKernel.Infrastructure.Persistence;
using Users.Domain.Entities;

namespace Users.Infrastructure.Persistence
{
    public class UsersDbContext : BaseDbContext
    {
        private readonly ITenantProvider? _tenantProvider;
        public DbSet<User> Users => Set<User>();

        // Constructor for runtime
        public UsersDbContext(
            DbContextOptions<UsersDbContext> options,
            ITenantProvider tenantProvider)
            : base(options, tenantProvider.GetCurrentTenantId())
        {
            _tenantProvider = tenantProvider;
        }

        // Constructor for design-time
        public UsersDbContext(DbContextOptions<UsersDbContext> options)
            : base(options, Guid.Empty)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("users");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);
        }
    }
}
