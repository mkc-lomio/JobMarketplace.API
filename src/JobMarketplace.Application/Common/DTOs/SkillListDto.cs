using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Common.DTOs
{
    public record SkillListDto
    {
        public Guid PublicGuid { get; init; }
        public long Id { get; init; }
        public string Name { get; init; } = default!;
        public string Description { get; init; } = default!;
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; set; }
    }
}
