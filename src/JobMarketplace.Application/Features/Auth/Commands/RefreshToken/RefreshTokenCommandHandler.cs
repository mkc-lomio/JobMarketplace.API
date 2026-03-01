using JobMarketplace.Application.Common.DTOs;
using JobMarketplace.Application.Common.Interfaces;
using JobMarketplace.Application.Common.Models;
using JobMarketplace.Application.Common;
using JobMarketplace.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ITokenService _tokenService;
        private readonly JwtSettings _jwtSettings;

        public RefreshTokenCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            ITokenService tokenService,
            IOptions<JwtSettings> jwtSettings)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _tokenService = tokenService;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);

            if (storedToken is null)
                return Result<AuthResponseDto>.Failure("Invalid refresh token.");

            // Theft detection: if a revoked token is reused, revoke ALL tokens for this user
            if (storedToken.IsRevoked)
            {
                await _refreshTokenRepository.RevokeAllUserTokensAsync(storedToken.UserId, cancellationToken);
                await _refreshTokenRepository.SaveChangesAsync(cancellationToken);
                return Result<AuthResponseDto>.Failure("Token has been revoked. All sessions terminated for security.");
            }

            if (storedToken.IsExpired)
                return Result<AuthResponseDto>.Failure("Refresh token has expired. Please log in again.");

            // Rotate: revoke old token, create new one
            storedToken.RevokedAt = DateTime.UtcNow;
            var newRefreshTokenString = _tokenService.GenerateRefreshToken();
            storedToken.ReplacedByToken = newRefreshTokenString;

            var newRefreshToken = new Domain.Entities.RefreshToken
            {
                Token = newRefreshTokenString,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
                UserId = storedToken.UserId
            };

            await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
            await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

            // Generate new access token
            var user = storedToken.User;
            var accessToken = _tokenService.GenerateAccessToken(user);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshTokenString,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
                UserPublicGuid = user.PublicGuid,
                Email = user.Email,
                Role = user.Role.ToString()
            });
        }
    }
}