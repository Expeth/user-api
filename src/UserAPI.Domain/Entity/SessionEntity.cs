using System;

namespace UserAPI.Domain.Entity
{
    public class SessionEntity
    {
        public string Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string UserId { get; set; }

        public SessionEntity(string id, DateTime creationTime, string userId, DateTime? endTime = null)
        {
            Id = id;
            StartTime = creationTime;
            UserId = userId;
        }
    }
}