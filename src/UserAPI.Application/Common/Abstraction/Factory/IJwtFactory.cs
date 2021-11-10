using System.Threading.Tasks;
using UserAPI.Domain.Entity;

namespace UserAPI.Application.Common.Abstraction.Factory
{
    public interface IJwtFactory
    {
        Task<string> CreateAsync(SessionEntity sessionEntity);
    }
}