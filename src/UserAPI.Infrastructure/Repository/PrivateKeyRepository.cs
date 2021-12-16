using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using UserAPI.Infrastructure.Abstraction;
using UserAPI.Infrastructure.Model;

namespace UserAPI.Infrastructure.Repository
{
    public class PrivateKeyRepository : IPrivateKeyRepository
    {
        private static readonly string PrivateKey = "privateKey";
        
        private readonly IMemoryCache _memoryCache;
        private readonly JwtCfg _jwtConfig;
        
        public PrivateKeyRepository(IMemoryCache memoryCache, IOptions<JwtCfg> jwtConfig)
        {
            _memoryCache = memoryCache;
            _jwtConfig = jwtConfig.Value;
        }

        public async Task<string> GetAsync()
        {
            return await _memoryCache.GetOrCreateAsync(PrivateKey, async e => await GetFromStorageAsync());
        }

        private Task<string> GetFromStorageAsync()
        {
            if (!File.Exists(_jwtConfig.PrivateKeyFileLocation))
            {
                throw new FileNotFoundException("There are no pem file for private key.");
            }
            
            return File.ReadAllTextAsync(_jwtConfig.PrivateKeyFileLocation);
        }
    }
}