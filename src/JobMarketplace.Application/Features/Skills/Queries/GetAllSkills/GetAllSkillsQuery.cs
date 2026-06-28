using JobMarketplace.Application.Common.DTOs;
using JobMarketplace.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Skills.Queries.GetAllSkills
{
    public record GetAllSkillsQuery(int PageSize = 20, long Cursor = 0) : IRequest<PagedResult<SkillListDto>>;
}
