using AutoMapper;
using JobMarketplace.Application.Common.Models;
using JobMarketplace.Domain.Entities;
using JobMarketplace.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Applications.Commands.CreateApplication
{
    public class CreateApplicationCommandHandler
        : IRequestHandler<CreateApplicationCommand, Result<Guid>>
    {
        private readonly IJobApplicationRepository _applicationRepository;
        private readonly IJobRepository _jobRepository;
        private readonly IMapper _mapper;

        public CreateApplicationCommandHandler(
            IJobApplicationRepository applicationRepository,
            IJobRepository jobRepository,
            IMapper mapper)
        {
            _applicationRepository = applicationRepository;
            _jobRepository = jobRepository;
            _mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(
            CreateApplicationCommand request, CancellationToken cancellationToken)
        {
            // Resolve the public GUID to the internal job and verify it's active
            var job = await _jobRepository.GetActiveJobByPublicGuidAsync(
                request.JobPublicGuid, cancellationToken);

            if (job is null)
                return Result<Guid>.Failure("Job not found or is no longer accepting applications.");

            var application = _mapper.Map<JobApplication>(request);
            application.JobId = job.Id;  // Set the internal FK
            application.AppliedAt = DateTime.UtcNow;

            await _applicationRepository.AddAsync(application, cancellationToken);
            await _applicationRepository.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(application.PublicGuid);
        }
    }
}
