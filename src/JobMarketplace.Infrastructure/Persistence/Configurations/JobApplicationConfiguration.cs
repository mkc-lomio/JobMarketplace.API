using JobMarketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Infrastructure.Persistence.Configurations
{
    public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
    {
        public void Configure(EntityTypeBuilder<JobApplication> builder)
        {
            // Primary Key: BIGINT IDENTITY(1,1), clustered
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id)
                .UseIdentityColumn();

            // Public GUID: UNIQUEIDENTIFIER with NEWSEQUENTIALID()
            builder.Property(a => a.PublicGuid)
                .HasDefaultValueSql("NEWSEQUENTIALID()")
                .IsRequired();
            builder.HasIndex(a => a.PublicGuid)
                .IsUnique();

            builder.Property(a => a.ApplicantName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(a => a.ApplicantEmail)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.Status)
                .HasConversion<string>()
                .HasMaxLength(30);

            // Relationships (FK uses internal BIGINT Id)
            builder.HasOne(a => a.Job)
                .WithMany(j => j.Applications)
                .HasForeignKey(a => a.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(a => a.JobId);
            builder.HasIndex(a => a.ApplicantEmail);
        }
    }
}
