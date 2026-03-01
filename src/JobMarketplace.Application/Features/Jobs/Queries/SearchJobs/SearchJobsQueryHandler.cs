using JobMarketplace.Application.Common.DTOs;
using JobMarketplace.Application.Common.Interfaces;
using JobMarketplace.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Jobs.Queries.SearchJobs
{
    public class SearchJobsQueryHandler : IRequestHandler<SearchJobsQuery, PagedResult<JobSearchDto>>
    {
        private readonly IDapperQueryService _queryService;

        public SearchJobsQueryHandler(IDapperQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<PagedResult<JobSearchDto>> Handle(SearchJobsQuery request, CancellationToken cancellationToken)
        {
            // Call SP — returns PageSize + 1 rows so PagedResult can detect if more pages exist
            var jobs = await _queryService.QueryAsync<JobSearchDto>(
                "sp_SearchJobs",
                new
                {
                    request.SearchTerm,       // Full-text search (SP auto-formats for CONTAINS)
                    request.Location,         // Optional filter
                    request.JobType,          // Optional filter
                    request.ExperienceLevel,  // Optional filter
                    request.PageSize,         // How many results per page
                    request.Cursor            // Last Id from previous page (0 = first page)
                },
                cancellationToken);

            // PagedResult trims the extra row, sets HasMore, and grabs NextCursor from the last item's Id
            return PagedResult<JobSearchDto>.Create(jobs.ToList(), request.PageSize, j => j.Id);
        }
    }
}