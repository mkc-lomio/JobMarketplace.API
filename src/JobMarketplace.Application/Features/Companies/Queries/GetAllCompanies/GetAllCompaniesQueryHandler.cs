using JobMarketplace.Application.Common.DTOs;
using JobMarketplace.Application.Common.Interfaces;
using JobMarketplace.Application.Common.Models;
using JobMarketplace.Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Companies.Queries.GetAllCompanies
{
    public class GetAllCompaniesQueryHandler : IRequestHandler<GetAllCompaniesQuery, PagedResult<CompanyListDto>>
    {
        private readonly IDapperQueryService _queryService;

        public GetAllCompaniesQueryHandler(IDapperQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<PagedResult<CompanyListDto>> Handle(GetAllCompaniesQuery request, CancellationToken cancellationToken)
        {
            var companies = await _queryService.QueryAsync<CompanyListDto>(
                "sp_GetAllCompanies",
                new { request.PageSize, request.Cursor },
                cancellationToken);

            return PagedResult<CompanyListDto>.Create(companies.ToList(), request.PageSize, c => c.Id);
        }
    }
}
