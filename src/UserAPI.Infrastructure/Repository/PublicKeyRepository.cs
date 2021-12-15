using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using UserAPI.Infrastructure.Abstraction;
using UserAPI.Infrastructure.Model;

namespace UserAPI.Infrastructure.Repository
{
    public class PublicKeyRepository : IPublicKeyRepository
    {
        private static readonly string PublicKey = "publicKey";
        
        private readonly IMemoryCache _memoryCache;
        private readonly JwtCfg _jwtConfig;

        public PublicKeyRepository(IMemoryCache memoryCache, IOptions<JwtCfg> jwtConfig)
        {
            _memoryCache = memoryCache;
            _jwtConfig = jwtConfig.Value;
        }

        public async Task<string> GetAsync()
        {
            return await _memoryCache.GetOrCreateAsync(PublicKey, async e => await GetFromStorageAsync());
        }

        private Task<string> GetFromStorageAsync()
        {
            if (!File.Exists(_jwtConfig.PublicKeyFileLocation))
            {
                throw new FileNotFoundException("There are no pem file for public key.");
            }
            
            return File.ReadAllTextAsync(_jwtConfig.PublicKeyFileLocation);
        }
    }
}