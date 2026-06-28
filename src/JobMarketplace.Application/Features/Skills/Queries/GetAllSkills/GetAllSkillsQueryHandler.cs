using JobMarketplace.Application.Common.DTOs;
using JobMarketplace.Application.Common.Interfaces;
using JobMarketplace.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Skills.Queries.GetAllSkills
{
    public class GetAllSkillsQueryHandler : IRequestHandler<GetAllSkillsQuery, PagedResult<SkillListDto>>
    {
        private readonly IDapperQueryService _queryService;

        public GetAllSkillsQueryHandler(IDapperQueryService queryService)
        {
            _queryService = queryService;
        }
        
        public async Task<PagedResult<SkillListDto>> Handle(GetAllSkillsQuery request, CancellationToken cancellationToken)
        {
            var skills = await _queryService.QueryAsync<SkillListDto>(
                "sp_GetAllSkills",
                new { request.PageSize, request.Cursor },
                cancellationToken);

            return PagedResult<SkillListDto>.Create(skills.ToList(), request.PageSize, s => s.Id);
        }
    }
}
