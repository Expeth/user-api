using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;
using UserAPI.Infrastructure.Abstraction;

namespace UserAPI.Infrastructure.Repository
{
    public class PrivateKeyRepository : IPrivateKeyRepository
    {
        private static readonly string PrivateKey = "privateKey";
        
        private readonly IMemoryCache _memoryCache;
        private readonly IMongoDatabase _database;

        public PrivateKeyRepository(IMongoDatabase database, IMemoryCache memoryCache)
        {
            _database = database;
            _memoryCache = memoryCache;
        }

        public async Task<string> GetPrivateKeyAsync()
        {
            return await _memoryCache.GetOrCreateAsync(PrivateKey, async e => await GetFromDbAsync());
        }

        private async Task<string> GetFromDbAsync()
        {
            var collection = _database.GetCollection<KeyDTO>("keys");
            var result = await collection.Find(i => i.Type == PrivateKey).FirstOrDefaultAsync();

            return result.Value;
        }

        private class KeyDTO
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public string Value { get; set; }
        }
    }
}