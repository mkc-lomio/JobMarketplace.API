using JobMarketplace.Application.Common.DTOs;
using JobMarketplace.Application.Common.Interfaces;  
using JobMarketplace.Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Companies.Queries.GetAllCompanies
{
    public class GetAllCompaniesQueryHandler : IRequestHandler<GetAllCompaniesQuery, List<CompanyDto>>
    {
        private readonly IDapperQueryService _queryService;

        public GetAllCompaniesQueryHandler(IDapperQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<List<CompanyDto>> Handle(GetAllCompaniesQuery request, CancellationToken cancellationToken)
        {
            // Dapper maps SP columns directly to CompanyDto — Id is excluded because the DTO doesn't have it
            var companies = await _queryService.QueryAsync<CompanyDto>(
                "sp_GetAllCompanies", cancellationToken: cancellationToken);
            return companies.ToList();
        }
    }
}
