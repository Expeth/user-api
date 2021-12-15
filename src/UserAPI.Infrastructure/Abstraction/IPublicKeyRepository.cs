using System.Threading.Tasks;

namespace UserAPI.Infrastructure.Abstraction
{
    public interface IPublicKeyRepository
    {
        Task<string> GetAsync();
    }
}