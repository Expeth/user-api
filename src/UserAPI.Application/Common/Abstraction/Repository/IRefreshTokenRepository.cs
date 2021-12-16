using System.Threading.Tasks;
using UserAPI.Domain.Entity;

namespace UserAPI.Application.Common.Abstraction.Repository
{
    public interface IRefreshTokenRepository
    {
        Task CreateAsync(RefreshTokenEntity entity);
        Task<bool> SetUsedAsync(string id);
        Task<RefreshTokenEntity> GetAsync(string id);
    }
}