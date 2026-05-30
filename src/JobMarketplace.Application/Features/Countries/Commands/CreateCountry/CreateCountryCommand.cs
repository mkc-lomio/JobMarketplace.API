using JobMarketplace.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Countries.Commands.CreateCountry
{
    public record CreateCountryCommand : IRequest<Result<Guid>>
    {
        public string FlagUrl { get; set; } = string.Empty;
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
