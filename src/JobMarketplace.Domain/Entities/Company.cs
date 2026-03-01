using JobMarketplace.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Domain.Entities
{
    public class Company : BaseAuditableEntity
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string? Website { get; set; }
        public string? LogoUrl { get; set; }
        public string Industry { get; set; } = default!;
        public string Location { get; set; } = default!;
        public int? EmployeeCount { get; set; }
        public int FoundedYear { get; set; }
        public string ContactEmail { get; set; } = default!;
        public string? ContactPhone { get; set; }

        // Navigation
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}
