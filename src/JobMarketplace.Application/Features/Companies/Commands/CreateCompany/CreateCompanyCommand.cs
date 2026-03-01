using JobMarketplace.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Companies.Commands.CreateCompany
{
    public record CreateCompanyCommand : IRequest<Result<Guid>>
    {
        public string Name { get; init; } = default!;
        public string Description { get; init; } = default!;
        public string? Website { get; init; }
        public string Industry { get; init; } = default!;
        public string Location { get; init; } = default!;
        public int? EmployeeCount { get; init; }
        public int FoundedYear { get; init; }
        public string ContactEmail { get; init; } = default!;
        public string? ContactPhone { get; init; }
    }
}
