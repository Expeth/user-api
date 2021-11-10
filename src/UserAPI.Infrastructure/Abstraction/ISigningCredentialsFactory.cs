using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace UserAPI.Infrastructure.Abstraction
{
    public interface ISigningCredentialsFactory
    {
        Task<SigningCredentials> CreateAsync();
    }
}