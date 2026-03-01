using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Common.DTOs
{
    /// <summary>
    /// Trimmed DTO for company list views — no Description, Website, LogoUrl, Phone.
    /// Full details still available via GetCompanyById (which uses CompanyDto).
    /// </summary>
    public record CompanyListDto
    {
        public long Id { get; init; }
        public Guid PublicGuid { get; init; }
        public string Name { get; init; } = default!;
        public string Industry { get; init; } = default!;
        public string Location { get; init; } = default!;
        public string ContactEmail { get; init; } = default!;
        public int FoundedYear { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
