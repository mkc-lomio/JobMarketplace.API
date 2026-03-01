using JobMarketplace.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Infrastructure.Services
{
    /// <summary>
    /// BCrypt wrapper. Work factor 11 = ~200ms per hash.
    /// Each call to Hash() produces a different result (random salt embedded in hash).
    /// Verify() extracts the salt from the stored hash and re-hashes to compare.
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        private const int WorkFactor = 11;

        public string Hash(string password)
            => BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);

        public bool Verify(string password, string hash)
            => BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
