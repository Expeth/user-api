using System;

namespace UserAPI.Domain.ValueObject
{
    public class JwtClaims
    {
        public string UserId { get; }
        public string SessionId { get; }
        public DateTime ExpiresAt { get; }
        public DateTime IssuedAt { get; }

        public JwtClaims(string userId, DateTime expiresAt, DateTime issuedAt, string sessionId)
        {
            UserId = userId;
            ExpiresAt = expiresAt;
            IssuedAt = issuedAt;
            SessionId = sessionId;
        }
    }
}