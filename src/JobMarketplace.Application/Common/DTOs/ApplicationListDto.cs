using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Common.DTOs
{
    /// <summary>
    /// Trimmed DTO for application list views — no CoverLetter, ResumeUrl, Notes, Phone.
    /// </summary>
    public record ApplicationListDto
    {
        public long Id { get; init; }
        public Guid PublicGuid { get; init; }
        public string ApplicantName { get; init; } = default!;
        public string ApplicantEmail { get; init; } = default!;
        public string Status { get; init; } = default!;
        public DateTime AppliedAt { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
