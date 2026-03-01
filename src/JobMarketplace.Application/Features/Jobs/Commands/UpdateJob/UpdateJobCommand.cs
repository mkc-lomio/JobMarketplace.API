using JobMarketplace.Application.Common.Models;
using JobMarketplace.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Jobs.Commands.UpdateJob
{
    public record UpdateJobCommand : IRequest<Result<bool>>
    {
        public Guid PublicGuid { get; init; }
        public string Title { get; init; } = default!;
        public string Description { get; init; } = default!;
        public string? Requirements { get; init; }
        public string Location { get; init; } = default!;
        public bool IsRemote { get; init; }
        public decimal? SalaryMin { get; init; }
        public decimal? SalaryMax { get; init; }
        public JobType JobType { get; init; }
        public ExperienceLevel ExperienceLevel { get; init; }
        public JobStatus Status { get; init; }
    }
}
