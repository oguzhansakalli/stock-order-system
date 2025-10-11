using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Domain.Entities;

namespace Orders.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);

            // OrderNumber value object
            builder.OwnsOne(o => o.OrderNumber, orderNumber =>
            {
                orderNumber.Property(on => on.Value)
                    .HasColumnName("OrderNumber")
                    .IsRequired()
                    .HasMaxLength(50);

                orderNumber.HasIndex(on => on.Value)
                    .IsUnique()
                    .HasDatabaseName("IX_Orders_OrderNumber");
            });

            builder.Property(o => o.CustomerId)
                .IsRequired();

            builder.Property(o => o.CustomerName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(o => o.Status)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(o => o.Notes)
                .HasMaxLength(1000);

            builder.Property(o => o.TenantId)
                .IsRequired();

            builder.HasIndex(o => o.TenantId)
                .HasDatabaseName("IX_Orders_TenantId");

            builder.HasIndex(o => o.CustomerId)
                .HasDatabaseName("IX_Orders_CustomerId");

            builder.HasIndex(o => o.Status)
                .HasDatabaseName("IX_Orders_Status");

            builder.Property(o => o.CreatedAt)
                .IsRequired();

            builder.Property(o => o.UpdatedAt);

            // OrderItems collection
            builder.OwnsMany(o => o.Items, items =>
            {
                items.ToTable("OrderItems", "orders");

                items.WithOwner().HasForeignKey("OrderId");

                items.Property<Guid>("Id")
                    .ValueGeneratedNever();

                items.HasKey("Id");

                items.Property(i => i.ProductId)
                    .IsRequired();

                items.Property(i => i.ProductName)
                    .IsRequired()
                    .HasMaxLength(200);

                items.Property(i => i.ProductSKU)
                    .IsRequired()
                    .HasMaxLength(50);

                items.Property(i => i.UnitPrice)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                items.Property(i => i.Quantity)
                    .IsRequired();

                items.Property(i => i.TotalPrice)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                items.HasIndex(i => i.ProductId)
                    .HasDatabaseName("IX_OrderItems_ProductId");
            });

            // Ignore domain events
            builder.Ignore(o => o.DomainEvents);
        }
    }
}
