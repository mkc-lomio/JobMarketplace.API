using AutoMapper;
using JobMarketplace.Application.Common.Models;
using JobMarketplace.Domain.Entities;
using JobMarketplace.Domain.Enums;
using JobMarketplace.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Jobs.Commands.CreateJob
{
    public class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, Result<Guid>>
    {
        private readonly IJobRepository _jobRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public CreateJobCommandHandler(
            IJobRepository jobRepository,
            ICompanyRepository companyRepository,
            IMapper mapper)
        {
            _jobRepository = jobRepository;
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(CreateJobCommand request, CancellationToken cancellationToken)
        {
            // Resolve the public GUID to the internal ID
            var company = await _companyRepository.GetByPublicGuidAsync(
                request.CompanyPublicGuid, cancellationToken);

            if (company is null)
                return Result<Guid>.Failure($"Company with Id '{request.CompanyPublicGuid}' not found.");

            var job = _mapper.Map<Job>(request);
            job.CompanyId = company.Id;  // Set the internal FK
            job.Status = JobStatus.Active;

            await _jobRepository.AddAsync(job, cancellationToken);
            await _jobRepository.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(job.PublicGuid);
        }
    }
}
