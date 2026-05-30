using JobMarketplace.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Domain.Entities
{
    public class Country : BaseAuditableEntity
    {
        public string FlagUrl { get; set; } = string.Empty;
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
