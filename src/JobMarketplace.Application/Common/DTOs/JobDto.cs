using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Common.DTOs
{
    public record JobDto
    {
        public Guid PublicGuid { get; init; }
        public string Title { get; init; } = default!;
        public string Description { get; init; } = default!;
        public string? Requirements { get; init; }
        public string? Responsibilities { get; init; }
        public string Location { get; init; } = default!;
        public bool IsRemote { get; init; }
        public decimal? SalaryMin { get; init; }
        public decimal? SalaryMax { get; init; }
        public string? SalaryCurrency { get; init; }
        public string JobType { get; init; } = default!;
        public string ExperienceLevel { get; init; } = default!;
        public string Status { get; init; } = default!;
        public DateTime? ExpiresAt { get; init; }
        public string? Tags { get; init; }
        public Guid CompanyPublicGuid { get; init; }
        public string CompanyName { get; init; } = default!;
        public DateTime CreatedAt { get; init; }
    }
}
