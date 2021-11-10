using System;

namespace UserAPI.Domain.Entity
{
    public class SessionEntity
    {
        public string Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string UserId { get; set; }

        public SessionEntity(string id, DateTime creationTime, string userId)
        {
            Id = id;
            CreationTime = creationTime;
            UserId = userId;
        }

        public SessionEntity(string userId)
        {
            Id = Guid.NewGuid().ToString();
            CreationTime = DateTime.UtcNow;
            UserId = userId;
        }
    }
}