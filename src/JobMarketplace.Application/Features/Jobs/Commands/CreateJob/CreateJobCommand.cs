using JobMarketplace.Application.Common.Models;
using JobMarketplace.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Jobs.Commands.CreateJob
{
    public record CreateJobCommand : IRequest<Result<Guid>>
    {
        public string Title { get; init; } = default!;
        public string Description { get; init; } = default!;
        public string? Requirements { get; init; }
        public string? Responsibilities { get; init; }
        public string Location { get; init; } = default!;
        public bool IsRemote { get; init; }
        public decimal? SalaryMin { get; init; }
        public decimal? SalaryMax { get; init; }
        public string? SalaryCurrency { get; init; }
        public JobType JobType { get; init; }
        public ExperienceLevel ExperienceLevel { get; init; }
        public string? Tags { get; init; }
        public Guid CompanyPublicGuid { get; init; }  // API sends the public GUID
    }
}
