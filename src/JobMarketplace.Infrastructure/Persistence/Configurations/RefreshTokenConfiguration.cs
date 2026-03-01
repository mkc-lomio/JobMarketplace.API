using JobMarketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Infrastructure.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(rt => rt.Id);
            builder.Property(rt => rt.Id).UseIdentityColumn();

            builder.Property(rt => rt.PublicGuid)
                .HasDefaultValueSql("NEWSEQUENTIALID()")
                .IsRequired();
            builder.HasIndex(rt => rt.PublicGuid).IsUnique();

            // Token — unique index for fast lookup during refresh
            builder.Property(rt => rt.Token).IsRequired().HasMaxLength(500);
            builder.HasIndex(rt => rt.Token).IsUnique();

            builder.Property(rt => rt.ExpiresAt).IsRequired();
            builder.Property(rt => rt.ReplacedByToken).HasMaxLength(500);

            // Computed properties — not mapped to DB columns
            builder.Ignore(rt => rt.IsExpired);
            builder.Ignore(rt => rt.IsRevoked);
            builder.Ignore(rt => rt.IsActive);
        }
    }
}
