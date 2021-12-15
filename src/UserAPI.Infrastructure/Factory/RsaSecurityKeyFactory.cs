using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using UserAPI.Infrastructure.Abstraction;

namespace UserAPI.Infrastructure.Factory
{
    public class RsaSecurityKeyFactory : ISigningCredentialsFactory, IDisposable
    {
        private readonly IPrivateKeyRepository _privateKeyRepository;
        private readonly RSA _rsa;
        
        public RsaSecurityKeyFactory(IPrivateKeyRepository privateKeyRepository)
        {
            _privateKeyRepository = privateKeyRepository;
            _rsa = RSA.Create();
        }

        public async Task<SigningCredentials> CreateAsync()
        {
            var privateKey = await _privateKeyRepository.GetAsync();
            _rsa.ImportFromPem(privateKey);

            return new SigningCredentials(key: new RsaSecurityKey(_rsa), algorithm: SecurityAlgorithms.RsaSha256);
        }

        public void Dispose()
        {
            _rsa?.Dispose();
        }
    }
}