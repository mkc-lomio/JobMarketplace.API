using JobMarketplace.Application.Common.Models;
using JobMarketplace.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Jobs.Commands.UpdateJob
{
    public class UpdateJobCommandHandler : IRequestHandler<UpdateJobCommand, Result<bool>>
    {
        private readonly IJobRepository _jobRepository;

        public UpdateJobCommandHandler(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<Result<bool>> Handle(UpdateJobCommand request, CancellationToken cancellationToken)
        {
            var job = await _jobRepository.GetByPublicGuidAsync(request.PublicGuid, cancellationToken);

            if (job is null)
                return Result<bool>.Failure($"Job with Id '{request.PublicGuid}' not found.");

            job.Title = request.Title;
            job.Description = request.Description;
            job.Requirements = request.Requirements;
            job.Location = request.Location;
            job.IsRemote = request.IsRemote;
            job.SalaryMin = request.SalaryMin;
            job.SalaryMax = request.SalaryMax;
            job.JobType = request.JobType;
            job.ExperienceLevel = request.ExperienceLevel;
            job.Status = request.Status;

            _jobRepository.Update(job);
            await _jobRepository.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
