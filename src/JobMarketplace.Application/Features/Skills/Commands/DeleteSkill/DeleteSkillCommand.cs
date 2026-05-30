using JobMarketplace.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Skills.Commands.DeleteSkill
{
    public record DeleteSkillCommand (Guid PublicGuid) : IRequest<Result<bool>>;
}
