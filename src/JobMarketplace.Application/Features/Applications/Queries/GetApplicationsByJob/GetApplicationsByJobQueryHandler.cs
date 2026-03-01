using JobMarketplace.Application.Common.DTOs;
using JobMarketplace.Application.Common.Interfaces;
using JobMarketplace.Application.Common.Models;
using MediatR;

namespace JobMarketplace.Application.Features.Applications.Queries.GetApplicationsByJob
{
    public class GetApplicationsByJobQueryHandler
           : IRequestHandler<GetApplicationsByJobQuery, PagedResult<ApplicationListDto>>
    {
        private readonly IDapperQueryService _queryService;

        public GetApplicationsByJobQueryHandler(IDapperQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<PagedResult<ApplicationListDto>> Handle(
            GetApplicationsByJobQuery request, CancellationToken cancellationToken)
        {
            var applications = await _queryService.QueryAsync<ApplicationListDto>(
                "sp_GetApplicationsByJobPublicGuid",
                new { request.JobPublicGuid, request.PageSize, request.Cursor },
                cancellationToken);

            return PagedResult<ApplicationListDto>.Create(applications.ToList(), request.PageSize, a => a.Id);
        }
    }
}