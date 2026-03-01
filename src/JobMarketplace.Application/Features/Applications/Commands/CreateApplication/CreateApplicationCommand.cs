using JobMarketplace.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Applications.Commands.CreateApplication
{
    public record CreateApplicationCommand : IRequest<Result<Guid>>
    {
        public Guid JobPublicGuid { get; init; }  // API sends the public GUID
        public string ApplicantName { get; init; } = default!;
        public string ApplicantEmail { get; init; } = default!;
        public string? ApplicantPhone { get; init; }
        public string? ResumeUrl { get; init; }
        public string? CoverLetter { get; init; }
    }
}
