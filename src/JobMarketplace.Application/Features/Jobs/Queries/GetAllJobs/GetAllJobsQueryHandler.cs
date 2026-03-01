using JobMarketplace.Application.Common.Interfaces;
using JobMarketplace.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Jobs.Queries.GetAllJobs
{
    public class GetAllJobsQueryHandler : IRequestHandler<GetAllJobsQuery, List<Job>>
    {
        private readonly IDapperQueryService _queryService;

        public GetAllJobsQueryHandler(IDapperQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<List<Job>> Handle(GetAllJobsQuery request, CancellationToken cancellationToken)
        {
            var jobs = await _queryService.QueryAsync<Job>(
                "sp_GetAllJobs", cancellationToken: cancellationToken);
            return jobs.ToList();
        }
    }
}
