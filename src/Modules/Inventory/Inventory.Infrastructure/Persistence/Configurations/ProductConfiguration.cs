using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.SKU)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(p => new { p.SKU, p.TenantId })
                .IsUnique()
                .HasDatabaseName("IX_Products_SKU_TenantId");

            builder.Property(p => p.Description)
                .HasMaxLength(1000);

            // Money value object mapping
            builder.OwnsOne(p => p.Price, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("Price")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("Currency")
                    .HasConversion<string>()
                    .HasMaxLength(3)
                    .IsRequired();
            });

            builder.Property(p => p.StockQuantity)
            .IsRequired();

            builder.Property(p => p.LowStockThreshold)
                .IsRequired()
                .HasDefaultValue(10);

            builder.Property(p => p.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(p => p.TenantId)
                .IsRequired();

            builder.HasIndex(p => p.TenantId)
                .HasDatabaseName("IX_Products_TenantId");

            builder.HasIndex(p => p.IsActive)
                .HasDatabaseName("IX_Products_IsActive");

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.Property(p => p.UpdatedAt);

            // Ignore domain events (not persisted)
            builder.Ignore(p => p.DomainEvents);
        }
    }
}
