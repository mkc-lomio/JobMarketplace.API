using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Common.ViewModels
{
    public record CreateSkillViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
