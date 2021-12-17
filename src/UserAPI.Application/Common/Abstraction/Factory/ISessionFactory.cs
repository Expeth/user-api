using UserAPI.Domain.Entity;

namespace UserAPI.Application.Common.Abstraction.Factory
{
    public interface ISessionFactory
    {
        SessionEntity Create(string userId);
    }
}