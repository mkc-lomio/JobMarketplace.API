using JobMarketplace.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Countries.Commands.DeleteCountry
{
    public record DeleteCountryCommand(Guid PublicGuid) : IRequest<Result<bool>>;
}
