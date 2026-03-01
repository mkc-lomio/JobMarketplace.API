using JobMarketplace.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Jobs.Commands.DeleteJob
{
    public record DeleteJobCommand(Guid PublicGuid) : IRequest<Result<bool>>;
}
