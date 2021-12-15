using System;

namespace UserAPI.Domain.ValueObject
{
    public class JwtClaims
    {
        public string UserId { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime IssuedAt { get; set; }

        public JwtClaims(string userId, DateTime expiresAt, DateTime issuedAt)
        {
            UserId = userId;
            ExpiresAt = expiresAt;
            IssuedAt = issuedAt;
        }
    }
}