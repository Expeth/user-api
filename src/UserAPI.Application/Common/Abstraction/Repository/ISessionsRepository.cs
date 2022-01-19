using System.Threading.Tasks;
using UserAPI.Domain.Entity;

namespace UserAPI.Application.Common.Abstraction.Repository
{
    public interface ISessionsRepository
    {
        Task<bool> SetEndedAsync(string id);
        Task<SessionEntity> GetAsync(string id);
        Task CreateAsync(SessionEntity entity);
    }
}