using JobMarketplace.Application.Common.Models;
using JobMarketplace.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Jobs.Commands.DeleteJob
{
    public class DeleteJobCommandHandler : IRequestHandler<DeleteJobCommand, Result<bool>>
    {
        private readonly IJobRepository _jobRepository;

        public DeleteJobCommandHandler(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<Result<bool>> Handle(DeleteJobCommand request, CancellationToken cancellationToken)
        {
            var job = await _jobRepository.GetByPublicGuidAsync(request.PublicGuid, cancellationToken);

            if (job is null)
                return Result<bool>.Failure($"Job with Id '{request.PublicGuid}' not found.");

            _jobRepository.Remove(job);
            await _jobRepository.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
