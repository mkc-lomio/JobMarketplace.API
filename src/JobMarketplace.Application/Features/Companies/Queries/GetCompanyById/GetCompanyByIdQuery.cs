using JobMarketplace.Application.Common.Models;
using JobMarketplace.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Companies.Queries.GetCompanyById
{
    public record GetCompanyByIdQuery(Guid PublicGuid) : IRequest<Result<Company>>;
}
