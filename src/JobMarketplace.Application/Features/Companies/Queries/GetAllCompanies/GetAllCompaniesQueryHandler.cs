using JobMarketplace.Domain.Entities;
using MediatR;
using JobMarketplace.Application.Common.Interfaces;  
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Companies.Queries.GetAllCompanies
{
    public class GetAllCompaniesQueryHandler : IRequestHandler<GetAllCompaniesQuery, List<Company>>
    {
        private readonly IDapperQueryService _queryService;

        public GetAllCompaniesQueryHandler(IDapperQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<List<Company>> Handle(GetAllCompaniesQuery request, CancellationToken cancellationToken)
        {
            var companies = await _queryService.QueryAsync<Company>(
                "sp_GetAllCompanies", cancellationToken: cancellationToken);
            return companies.ToList();
        }
    }
}
