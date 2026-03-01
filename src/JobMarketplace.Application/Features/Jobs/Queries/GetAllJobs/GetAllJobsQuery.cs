using JobMarketplace.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Jobs.Queries.GetAllJobs
{
    public record GetAllJobsQuery : IRequest<List<Job>>;
}
