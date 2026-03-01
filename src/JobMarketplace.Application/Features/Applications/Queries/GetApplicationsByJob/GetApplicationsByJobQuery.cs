using JobMarketplace.Application.Common.DTOs;
using JobMarketplace.Application.Common.Models;
using JobMarketplace.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Applications.Queries.GetApplicationsByJob
{
    public record GetApplicationsByJobQuery(Guid JobPublicGuid, int PageSize = 20, long Cursor = 0)
         : IRequest<PagedResult<ApplicationListDto>>;
}
