using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Common.ViewModels
{
    public record CreateTransactionViewModel
    {
        public Guid CountryId {  get; set; }
        public Guid SkillId { get; set; }
    }
}
