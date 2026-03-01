using JobMarketplace.Application.Common.DTOs;
using JobMarketplace.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Applications.Queries.GetApplicationsByJob
{
    public record GetApplicationsByJobQuery(Guid JobPublicGuid) : IRequest<List<JobApplicationDto>>;
}
