using JobMarketplace.Application.Common.DTOs;
using JobMarketplace.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Jobs.Queries.SearchJobs
{
    public record SearchJobsQuery : IRequest<PagedResult<JobSearchDto>>
    {
        public string? SearchTerm { get; init; }
        public string? Location { get; init; }
        public string? JobType { get; init; }
        public string? ExperienceLevel { get; init; }
        public int PageSize { get; init; } = 20;
        public long Cursor { get; init; } = 0;
    }
}
