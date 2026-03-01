using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Common
{
    /// <summary>Bound from appsettings.json "JwtSettings" section.</summary>
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";
        public string SecretKey { get; set; } = default!;
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public int AccessTokenExpiryMinutes { get; set; } = 15;
        public int RefreshTokenExpiryDays { get; set; } = 7;
    }
}
