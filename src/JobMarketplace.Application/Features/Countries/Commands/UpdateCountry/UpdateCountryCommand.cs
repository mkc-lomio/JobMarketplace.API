using JobMarketplace.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Countries.Commands.UpdateCountry
{
    public record UpdateCountryCommand : IRequest<Result<bool>>
    {
        public Guid PublicGuid { get; init; }
        public string FlagUrl { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
    }
}
