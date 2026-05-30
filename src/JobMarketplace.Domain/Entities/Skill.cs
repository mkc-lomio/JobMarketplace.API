using JobMarketplace.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Domain.Entities
{
    public class Skill : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}
