using JobMarketplace.Application.Common.DTOs;
using JobMarketplace.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Auth.Commands.RefreshToken
{
    public record RefreshTokenCommand : IRequest<Result<AuthResponseDto>>
    {
        public string RefreshToken { get; init; } = default!;
    }
}
