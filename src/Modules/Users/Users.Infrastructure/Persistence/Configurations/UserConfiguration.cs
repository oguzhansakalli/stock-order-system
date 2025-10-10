using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Entities;

namespace Users.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            // Email value object mapping
            builder.OwnsOne(u => u.Email, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("Email")
                    .IsRequired()
                    .HasMaxLength(255);

                email.HasIndex(e => e.Value)
                    .HasDatabaseName("IX_Users_Email");
            });

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.Role)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(u => u.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(u => u.LastLoginAt);

            builder.Property(u => u.TenantId)
                .IsRequired();

            builder.HasIndex(u => u.TenantId)
                .HasDatabaseName("IX_Users_TenantId");

            builder.HasIndex(u => u.IsActive)
                .HasDatabaseName("IX_Users_IsActive");

            builder.Property(u => u.CreatedAt)
                .IsRequired();

            builder.Property(u => u.UpdatedAt);

            // RefreshToken collection mapping
            builder.OwnsMany(u => u.RefreshTokens, token =>
            {
                token.ToTable("RefreshTokens", "users");

                token.WithOwner().HasForeignKey("UserId");

                token.Property<int>("Id")
                    .ValueGeneratedOnAdd();

                token.HasKey("Id");

                token.Property(t => t.Token)
                    .IsRequired()
                    .HasMaxLength(500);

                token.Property(t => t.ExpiresAt)
                    .IsRequired();

                token.Property(t => t.CreatedAt)
                    .IsRequired();

                token.Property(t => t.IsRevoked)
                    .IsRequired();

                token.HasIndex(t => t.Token)
                    .HasDatabaseName("IX_RefreshTokens_Token");
            });

            // Ignore domain events
            builder.Ignore(u => u.DomainEvents);
        }
    }
}
