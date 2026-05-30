using JobMarketplace.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Skills.Commands.UpdateSkill
{
    public class UpdateSkillCommand : IRequest<Result<bool>>
    {
        public Guid PublicGuid { get; init; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
    }
}
