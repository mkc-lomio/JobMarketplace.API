using JobMarketplace.Application.Common.DTOs;
using JobMarketplace.Application.Common.Interfaces;
using JobMarketplace.Application.Common.Models;
using JobMarketplace.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Companies.Queries.GetCompanyById
{
    public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, Result<CompanyDto>>
    {
        private readonly IDapperQueryService _queryService;

        public GetCompanyByIdQueryHandler(IDapperQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<Result<CompanyDto>> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
        {
            var company = await _queryService.QueryFirstOrDefaultAsync<CompanyDto>(
                "sp_GetCompanyByPublicGuid",
                new { PublicGuid = request.PublicGuid },
                cancellationToken);

            return company is null
                ? Result<CompanyDto>.Failure($"Company with Id '{request.PublicGuid}' not found.")
                : Result<CompanyDto>.Success(company);
        }
    }
}
