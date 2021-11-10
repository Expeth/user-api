using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using UserAPI.Application.Common.Abstraction.Service;
using UserAPI.Domain.ValueObject;

namespace UserAPI.Infrastructure.Service
{
    public class Pbkdf2PasswordService : IPasswordService
    {
        private static readonly int SaltBits = 128;
        private static readonly int IterationsCount = 100_000;
        private static readonly int HashBits = 512;
        
        public PasswordHash GenerateHash(string password, string salt = null)
        {
            var pwdSaltBytes = string.IsNullOrEmpty(salt) ? GenerateSalt() : Convert.FromBase64String(salt);
            var pwdHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password,
                pwdSaltBytes,
                KeyDerivationPrf.HMACSHA256,
                IterationsCount,
                HashBits / 8));
            
            return new PasswordHash(pwdHash, Convert.ToBase64String(pwdSaltBytes));
        }

        public bool ValidateHash(string hash, string password, string salt)
        {
            var pwdHash = GenerateHash(password, salt);
            return string.Equals(pwdHash.Hash, hash, StringComparison.OrdinalIgnoreCase);
        }

        private byte[] GenerateSalt()
        {
            var salt = new byte[SaltBits / 8];
            using var rngCsp = new RNGCryptoServiceProvider();
            rngCsp.GetNonZeroBytes(salt);
            
            return salt;
        }
    }
}