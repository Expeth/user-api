using System;
using UserAPI.Application.Common.Abstraction.Factory;
using UserAPI.Domain.Entity;

namespace UserAPI.Infrastructure.Factory
{
    public class SessionFactory : ISessionFactory
    {
        public SessionEntity Create(string userId)
        {
            return new SessionEntity(Guid.NewGuid().ToString(), DateTime.UtcNow, userId);
        }
    }
}