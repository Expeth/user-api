using System.Threading.Tasks;
using UserAPI.Domain.Entity;

namespace UserAPI.Application.Common.Abstraction.Repository
{
    public interface IUserRepository
    {
        Task<bool> CreateAsync(UserEntity entity);
        Task<UserEntity> GetAsync(string login);
    }
}