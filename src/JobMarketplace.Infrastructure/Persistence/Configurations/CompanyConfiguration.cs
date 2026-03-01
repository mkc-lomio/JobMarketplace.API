using JobMarketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Infrastructure.Persistence.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            // Primary Key: BIGINT IDENTITY(1,1), clustered
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .UseIdentityColumn();

            // Public GUID: UNIQUEIDENTIFIER with NEWSEQUENTIALID()
            builder.Property(c => c.PublicGuid)
                .HasDefaultValueSql("NEWSEQUENTIALID()")
                .IsRequired();
            builder.HasIndex(c => c.PublicGuid)
                .IsUnique();

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Description)
                .IsRequired();

            builder.Property(c => c.Industry)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Location)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.ContactEmail)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Website)
                .HasMaxLength(500);

            builder.HasIndex(c => c.Name);
        }
    }
}
