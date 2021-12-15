using System;
using UserAPI.Application.Common.Abstraction.Factory;
using UserAPI.Domain.Entity;

namespace UserAPI.Infrastructure.Factory
{
    public class RefreshTokenFactory : IRefreshTokenFactory
    {
        public RefreshTokenEntity Create(string userId)
        {
            return new RefreshTokenEntity(Guid.NewGuid().ToString(),
                DateTime.UtcNow, DateTime.UtcNow.AddDays(1), userId);
        }
    }
}