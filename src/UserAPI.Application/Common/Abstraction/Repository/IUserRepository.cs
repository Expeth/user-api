using System.Threading.Tasks;
using UserAPI.Domain.Entity;

namespace UserAPI.Application.Common.Abstraction.Repository
{
    public interface IUserRepository
    {
        Task<bool> CreateUserAsync(UserEntity entity);
        Task<UserEntity> GetUserAsync(string login);
    }
}