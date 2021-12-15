using UserAPI.Domain.Entity;

namespace UserAPI.Application.Common.Abstraction.Factory
{
    public interface IRefreshTokenFactory
    {
        RefreshTokenEntity Create(string userId);
    }
}