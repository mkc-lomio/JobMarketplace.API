using JobMarketplace.Application.Common.DTOs;
using JobMarketplace.Application.Common.Interfaces;
using JobMarketplace.Application.Common.Models;
using JobMarketplace.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Jobs.Queries.GetJobById
{
    public class GetJobByIdQueryHandler : IRequestHandler<GetJobByIdQuery, Result<JobDto>>
    {
        private readonly IDapperQueryService _queryService;

        public GetJobByIdQueryHandler(IDapperQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<Result<JobDto>> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
        {
            var job = await _queryService.QueryFirstOrDefaultAsync<JobDto>(
                "sp_GetJobByPublicGuid",
                new { PublicGuid = request.PublicGuid },
                cancellationToken);

            return job is null
                ? Result<JobDto>.Failure($"Job with Id '{request.PublicGuid}' not found.")
                : Result<JobDto>.Success(job);
        }
    }
}
