using System.Threading.Tasks;

namespace UserAPI.Infrastructure.Abstraction
{
    public interface IPrivateKeyRepository
    {
        Task<string> GetPrivateKeyAsync();
    }
}