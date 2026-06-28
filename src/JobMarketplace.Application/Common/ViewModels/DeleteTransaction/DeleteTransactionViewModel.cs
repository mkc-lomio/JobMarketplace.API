using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Common.ViewModels.DeleteTransaction
{
    public record DeleteTransactionViewModel
    {
        public Guid CountryId { get; set; }
        public Guid SkillId { get; set; }
    }
}
