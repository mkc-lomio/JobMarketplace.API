using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Common.DTOs
{
    public record CompanyDto
    {
        public Guid PublicGuid { get; init; }
        public string Name { get; init; } = default!;
        public string Description { get; init; } = default!;
        public string? Website { get; init; }
        public string? LogoUrl { get; init; }
        public string Industry { get; init; } = default!;
        public string Location { get; init; } = default!;
        public int? EmployeeCount { get; init; }
        public int FoundedYear { get; init; }
        public string ContactEmail { get; init; } = default!;
        public string? ContactPhone { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? LastModifiedAt { get; init; }
    }
}
