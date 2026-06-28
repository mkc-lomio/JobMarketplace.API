using JobMarketplace.Application.Common.Models;
using JobMarketplace.Application.Common.ViewModels.DeleteTransaction;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Transactions.Commands.DeleteTransaction
{
    public record DeleteTransactionCommand : IRequest<Result<DeleteTransactionViewModel>>
    {
        public Guid CountryId { get; set; }
        public Guid SkillId { get; set; }

    }
}
