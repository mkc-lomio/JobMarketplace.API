using JobMarketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Infrastructure.Persistence.Configurations
{
    public class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            // Primary Key: BIGINT IDENTITY(1,1), clustered
            builder.HasKey(j => j.Id);
            builder.Property(j => j.Id)
                .UseIdentityColumn();

            // Public GUID: UNIQUEIDENTIFIER with NEWSEQUENTIALID()
            builder.Property(j => j.PublicGuid)
                .HasDefaultValueSql("NEWSEQUENTIALID()")
                .IsRequired();
            builder.HasIndex(j => j.PublicGuid)
                .IsUnique();

            builder.Property(j => j.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(j => j.Description)
                .IsRequired();

            builder.Property(j => j.Location)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(j => j.SalaryMin)
                .HasColumnType("decimal(18,2)");

            builder.Property(j => j.SalaryMax)
                .HasColumnType("decimal(18,2)");

            builder.Property(j => j.SalaryCurrency)
                .HasMaxLength(3);

            builder.Property(j => j.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(j => j.JobType)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(j => j.ExperienceLevel)
                .HasConversion<string>()
                .HasMaxLength(20);

            // Relationships (FK uses internal BIGINT Id)
            builder.HasOne(j => j.Company)
                .WithMany(c => c.Jobs)
                .HasForeignKey(j => j.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(j => j.CompanyId);
            builder.HasIndex(j => j.Status);
        }
    }
}
