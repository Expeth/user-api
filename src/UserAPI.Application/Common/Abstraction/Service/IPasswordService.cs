using UserAPI.Domain.ValueObject;

namespace UserAPI.Application.Common.Abstraction.Service
{
    public interface IPasswordService
    {
        PasswordHash GenerateHash(string password, string salt = null);
        bool ValidateHash(string hash, string password, string salt);
    }
}