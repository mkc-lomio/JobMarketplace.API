using JobMarketplace.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Companies.Queries.GetAllCompanies
{
    public record GetAllCompaniesQuery : IRequest<List<Company>>;
}
