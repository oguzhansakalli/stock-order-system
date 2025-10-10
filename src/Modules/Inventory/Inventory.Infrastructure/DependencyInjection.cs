using Inventory.Domain.Repositories;
using Inventory.Infrastructure.Persistence;
using Inventory.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Application.Abstractions;

namespace Inventory.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInventoryInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<InventoryDbContext>((serviceProvider, options) =>
            {
                var tenantProvider = serviceProvider.GetRequiredService<ITenantProvider>();
                var tenantId = tenantProvider.GetCurrentTenantId();

                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b =>
                    {
                        b.MigrationsAssembly(typeof(InventoryDbContext).Assembly.FullName);
                        b.MigrationsHistoryTable("__EFMigrationsHistory", "inventory");
                    });

                // Enable sensitive data logging in development
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (environment == "Development")
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
            });

            // Register repositories
            services.AddScoped<IProductRepository, ProductRepository>();

            // Register Keyed UnitOfWork
            services.AddKeyedScoped<IUnitOfWork, InventoryDbContext>(
                "Inventory",
                (sp, key) => sp.GetRequiredService<InventoryDbContext>()
            );

            return services;
        }
    }
}
