using AutoMapper;
using JobMarketplace.Application.Features.Applications.Commands.CreateApplication;
using JobMarketplace.Application.Features.Companies.Commands.CreateCompany;
using JobMarketplace.Application.Features.Jobs.Commands.CreateJob;
using JobMarketplace.Application.Features.Jobs.Commands.UpdateJob;
using JobMarketplace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Company
            CreateMap<CreateCompanyCommand, Company>();

            // Job
            CreateMap<CreateJobCommand, Job>()
                .ForMember(dest => dest.CompanyId, opt => opt.Ignore());  // Set manually in handler

            CreateMap<UpdateJobCommand, Job>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PublicGuid, opt => opt.Ignore())
                .ForMember(dest => dest.CompanyId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());

            // Application
            CreateMap<CreateApplicationCommand, JobApplication>()
                .ForMember(dest => dest.JobId, opt => opt.Ignore());  // Set manually in handler
        }
    }
}
