using JobMarketplace.Application.Common.Models;
using JobMarketplace.Application.Common.ViewModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Transactions.Commands
{
    public record CreateTransactionCommand : IRequest<Result<CreateTransactionViewModel>>
    {
        public CreateCountryViewModel CountryViewModel  { get; set;}
        
        public CreateSkillViewModel SkillViewModel { get; set;}
    }
}
