using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Common.DTOs
{
    public record JobApplicationDto
    {
        public Guid PublicGuid { get; init; }
        public string ApplicantName { get; init; } = default!;
        public string ApplicantEmail { get; init; } = default!;
        public string? ApplicantPhone { get; init; }
        public string? ResumeUrl { get; init; }
        public string? CoverLetter { get; init; }
        public string Status { get; init; } = default!;
        public string? Notes { get; init; }
        public DateTime AppliedAt { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
