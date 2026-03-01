using JobMarketplace.Domain.Common;
using JobMarketplace.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Domain.Entities
{
    public class JobApplication : BaseAuditableEntity
    {
        public string ApplicantName { get; set; } = default!;
        public string ApplicantEmail { get; set; } = default!;
        public string? ApplicantPhone { get; set; }
        public string? ResumeUrl { get; set; }
        public string? CoverLetter { get; set; }
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Submitted;
        public string? Notes { get; set; }
        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys (internal — never exposed in API)
        public long JobId { get; set; }

        // Navigation
        public Job Job { get; set; } = default!;
    }
}
