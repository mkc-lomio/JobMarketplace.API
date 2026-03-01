using JobMarketplace.Application.Common.Models;
using JobMarketplace.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Jobs.Queries.GetJobById
{
    public record GetJobByIdQuery(Guid PublicGuid) : IRequest<Result<Job>>;
}
