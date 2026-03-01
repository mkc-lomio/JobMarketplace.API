using JobMarketplace.Application.Common.DTOs;
using JobMarketplace.Application.Common.Interfaces;
using JobMarketplace.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Jobs.Queries.GetAllJobs
{
    public class GetAllJobsQueryHandler : IRequestHandler<GetAllJobsQuery, List<JobDto>>
    {
        private readonly IDapperQueryService _queryService;

        public GetAllJobsQueryHandler(IDapperQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<List<JobDto>> Handle(GetAllJobsQuery request, CancellationToken cancellationToken)
        {
            var jobs = await _queryService.QueryAsync<JobDto>(
                "sp_GetAllJobs", cancellationToken: cancellationToken);
            return jobs.ToList();
        }
    }
}
