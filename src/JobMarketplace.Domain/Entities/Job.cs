using JobMarketplace.Domain.Common;
using JobMarketplace.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Domain.Entities
{
    public class Job : BaseAuditableEntity
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string? Requirements { get; set; }
        public string? Responsibilities { get; set; }
        public string Location { get; set; } = default!;
        public bool IsRemote { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public string? SalaryCurrency { get; set; }
        public JobType JobType { get; set; }
        public ExperienceLevel ExperienceLevel { get; set; }
        public JobStatus Status { get; set; } = JobStatus.Draft;
        public DateTime? ExpiresAt { get; set; }
        public string? Tags { get; set; }

        // Foreign Keys (internal — never exposed in API)
        public long CompanyId { get; set; }

        // Navigation
        public Company Company { get; set; } = default!;
        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    }
}
