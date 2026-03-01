using JobMarketplace.Application.Common.DTOs;
using JobMarketplace.Application.Common.Models;
using JobMarketplace.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Auth.Commands.Register
{
    public record RegisterCommand : IRequest<Result<AuthResponseDto>>
    {
        public string Email { get; init; } = default!;
        public string Password { get; init; } = default!;
        public string FirstName { get; init; } = default!;
        public string LastName { get; init; } = default!;
        public UserRole Role { get; init; } = UserRole.JobSeeker;
    }
}
