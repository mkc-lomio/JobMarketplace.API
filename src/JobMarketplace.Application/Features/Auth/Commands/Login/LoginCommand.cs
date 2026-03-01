using JobMarketplace.Application.Common.DTOs;
using JobMarketplace.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Auth.Commands.Login
{
    public record LoginCommand : IRequest<Result<AuthResponseDto>>
    {
        public string Email { get; init; } = default!;
        public string Password { get; init; } = default!;
    }
}
