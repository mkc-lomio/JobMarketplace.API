using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Common.DTOs
{
    /// <summary>Returned by Register, Login, and RefreshToken endpoints.</summary>
    public record AuthResponseDto
    {
        public string AccessToken { get; init; } = default!;
        public string RefreshToken { get; init; } = default!;
        public DateTime AccessTokenExpiresAt { get; init; }
        public Guid UserPublicGuid { get; init; }
        public string Email { get; init; } = default!;
        public string Role { get; init; } = default!;
    }
}
