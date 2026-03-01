using JobMarketplace.Application.Common.DTOs;
using JobMarketplace.Application.Common.Interfaces;
using JobMarketplace.Application.Common.Models;
using JobMarketplace.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Jobs.Queries.GetAllJobs
{
    public class GetAllJobsQueryHandler : IRequestHandler<GetAllJobsQuery, PagedResult<JobListDto>>
    {
        private readonly IDapperQueryService _queryService;

        public GetAllJobsQueryHandler(IDapperQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<PagedResult<JobListDto>> Handle(GetAllJobsQuery request, CancellationToken cancellationToken)
        {
            var jobs = await _queryService.QueryAsync<JobListDto>(
                "sp_GetAllJobs",
                new { request.PageSize, request.Cursor },
                cancellationToken);

            return PagedResult<JobListDto>.Create(jobs.ToList(), request.PageSize, j => j.Id);
        }
    }
}
