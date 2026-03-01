using JobMarketplace.Domain.Common;
using JobMarketplace.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Domain.Entities
{
    /// <summary>
    /// Represents a registered user. Password is stored as a BCrypt hash — never plain text.
    /// FailedLoginAttempts + LockoutEnd provide brute-force protection.
    /// </summary>
    public class User : BaseAuditableEntity
    {
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public UserRole Role { get; set; } = UserRole.JobSeeker;
        public bool IsActive { get; set; } = true;
        public int FailedLoginAttempts { get; set; } = 0;
        public DateTime? LockoutEnd { get; set; }

        // Navigation
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        // Helpers
        public bool IsLockedOut => LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;
    }
}