using JobMarketplace.Application.Common.DTOs;
using JobMarketplace.Application.Common.Interfaces;  
using JobMarketplace.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Applications.Queries.GetApplicationsByJob
{
    public class GetApplicationsByJobQueryHandler
           : IRequestHandler<GetApplicationsByJobQuery, List<JobApplicationDto>>
    {
        private readonly IDapperQueryService _queryService;

        public GetApplicationsByJobQueryHandler(IDapperQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<List<JobApplicationDto>> Handle(
            GetApplicationsByJobQuery request, CancellationToken cancellationToken)
        {
            var applications = await _queryService.QueryAsync<JobApplicationDto>(
                "sp_GetApplicationsByJobPublicGuid",
                new { JobPublicGuid = request.JobPublicGuid },
                cancellationToken);
            return applications.ToList();
        }
    }
}
