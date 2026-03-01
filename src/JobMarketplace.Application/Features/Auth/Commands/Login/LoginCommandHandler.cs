using JobMarketplace.Application.Common.DTOs;
using JobMarketplace.Application.Common.Interfaces;
using JobMarketplace.Application.Common.Models;
using JobMarketplace.Application.Common;
using JobMarketplace.Domain.Entities;
using JobMarketplace.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly JwtSettings _jwtSettings;

        private const int MaxFailedAttempts = 5;
        private const int LockoutMinutes = 3;

        public LoginCommandHandler(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IOptions<JwtSettings> jwtSettings)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email.ToLowerInvariant(), cancellationToken);

            if (user is null)
                return Result<AuthResponseDto>.Failure("Invalid email or password.");

            if (!user.IsActive)
                return Result<AuthResponseDto>.Failure("Account is deactivated.");

            if (user.IsLockedOut)
                return Result<AuthResponseDto>.Failure($"Account is locked. Try again after {user.LockoutEnd:HH:mm} UTC.");

            if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= MaxFailedAttempts)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(LockoutMinutes);
                    user.FailedLoginAttempts = 0;
                }

                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync(cancellationToken);
                return Result<AuthResponseDto>.Failure("Invalid email or password.");
            }

            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            _userRepository.Update(user);

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshTokenString = _tokenService.GenerateRefreshToken();

            var refreshToken = new Domain.Entities.RefreshToken
            {
                Token = refreshTokenString,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
                UserId = user.Id
            };

            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
            await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenString,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
                UserPublicGuid = user.PublicGuid,
                Email = user.Email,
                Role = user.Role.ToString()
            });
        }
    }
}