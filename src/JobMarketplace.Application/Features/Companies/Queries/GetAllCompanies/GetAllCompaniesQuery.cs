using JobMarketplace.Application.Common.DTOs;
using JobMarketplace.Application.Common.Models;
using JobMarketplace.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Companies.Queries.GetAllCompanies
{
    public record GetAllCompaniesQuery(int PageSize = 20, long Cursor = 0) : IRequest<PagedResult<CompanyListDto>>;
}
