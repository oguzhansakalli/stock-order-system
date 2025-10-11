using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Domain.Repositories;
using Orders.Infrastructure.Persistence;
using Orders.Infrastructure.Repositories;
using SharedKernel.Application.Abstractions;

namespace Orders.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddOrdersInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<OrdersDbContext>((serviceProvider, options) =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b =>
                    {
                        b.MigrationsAssembly(typeof(OrdersDbContext).Assembly.FullName);
                        b.MigrationsHistoryTable("__EFMigrationsHistory", "orders");
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
            services.AddScoped<IOrderRepository, OrderRepository>();

            // Register Keyed UnitOfWork for Orders
            services.AddKeyedScoped<IUnitOfWork, OrdersDbContext>(
                "Orders",
                (sp, key) => sp.GetRequiredService<OrdersDbContext>()
            );

            return services;
        }
    }
}
