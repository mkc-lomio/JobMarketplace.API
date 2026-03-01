using JobMarketplace.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Domain.Entities
{
    /// <summary>
    /// Stored in DB. On each refresh, the old token is revoked and replaced by a new one (rotation).
    /// If a revoked token is used again, ALL tokens for that user are revoked (theft detection).
    /// </summary>
    public class RefreshToken : BaseAuditableEntity
    {
        public string Token { get; set; } = default!;
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? ReplacedByToken { get; set; }

        // Computed — not stored in DB
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt != null;
        public bool IsActive => !IsRevoked && !IsExpired;

        // Foreign key (internal BIGINT — never exposed)
        public long UserId { get; set; }
        public User User { get; set; } = default!;
    }
}
