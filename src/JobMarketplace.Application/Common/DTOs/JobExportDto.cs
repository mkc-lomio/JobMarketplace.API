using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Common.DTOs
{
    /// <summary>
    /// Lightweight DTO for streaming export — no Id (not needed by consumers).
    /// </summary>
    public record JobExportDto
    {
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
        public string CompanyName { get; init; } = default!;
        public string CompanyIndustry { get; init; } = default!;
    }
}
