using JobMarketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).UseIdentityColumn();

            builder.Property(u => u.PublicGuid)
                .HasDefaultValueSql("NEWSEQUENTIALID()")
                .IsRequired();
            builder.HasIndex(u => u.PublicGuid).IsUnique();

            // Email — unique, case-insensitive lookups via index
            builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(200);
            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);

            // Role stored as string ("JobSeeker", "Employer", "Admin")
            builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(50);

            builder.Property(u => u.IsActive).HasDefaultValue(true);
            builder.Property(u => u.FailedLoginAttempts).HasDefaultValue(0);

            // One user → many refresh tokens
            builder.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
