using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Common.DTOs
{
    /// <summary>
    /// Trimmed DTO for job list views — no Description, Requirements, Responsibilities, Tags.
    /// Full details are still available via GetJobById (which uses JobDto).
    /// </summary>
    public record JobListDto
    {
        public long Id { get; init; }
        public Guid PublicGuid { get; init; }
        public string Title { get; init; } = default!;
        public string Location { get; init; } = default!;
        public bool IsRemote { get; init; }
        public decimal? SalaryMin { get; init; }
        public decimal? SalaryMax { get; init; }
        public string? SalaryCurrency { get; init; }
        public string JobType { get; init; } = default!;
        public string ExperienceLevel { get; init; } = default!;
        public DateTime CreatedAt { get; init; }
        public Guid CompanyPublicGuid { get; init; }
        public string CompanyName { get; init; } = default!;
    }
}
