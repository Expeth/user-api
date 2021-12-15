using System;

namespace UserAPI.Domain.Entity
{
    public class RefreshTokenEntity
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public bool IsDeclined { get; set; }

        public RefreshTokenEntity(string id, DateTime issuedAt, DateTime expiresAt, string userId, bool isUsed = false,
            bool isDeclined = false)
        {
            Id = id;
            IssuedAt = issuedAt;
            ExpiresAt = expiresAt;
            UserId = userId;
            IsUsed = isUsed;
            IsDeclined = isDeclined;
        }
    }
}