using System.Threading.Tasks;
using MongoDB.Driver;
using UserAPI.Application.Common.Abstraction.Repository;
using UserAPI.Domain.Entity;

namespace UserAPI.Infrastructure.Repository
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IMongoDatabase _database;

        public RefreshTokenRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task<bool> CreateAsync(RefreshTokenEntity entity)
        {
            var collection = _database.GetCollection<RefreshTokenEntity>("refreshTokens");

            await collection.InsertOneAsync(entity);
            return true;
        }

        public async Task<bool> SetUsedAsync(string id)
        {
            var collection = _database.GetCollection<RefreshTokenEntity>("refreshTokens");
            var filterBuilder = Builders<RefreshTokenEntity>.Filter;
            var filter = filterBuilder.Eq(i => i.Id, id);

            var updateBuilder = Builders<RefreshTokenEntity>.Update;
            var update = updateBuilder.Set(i => i.IsUsed, true);

            var result = await collection.UpdateOneAsync(filter, update);
            return result.IsAcknowledged;
        }

        public async Task<RefreshTokenEntity> GetAsync(string id)
        {
            var collection = _database.GetCollection<RefreshTokenEntity>("refreshTokens");
            var builder = Builders<RefreshTokenEntity>.Filter;
            var filter = builder.Eq(i => i.Id, id);
            
            var entity = await collection.Find(filter).FirstOrDefaultAsync();
            return entity;
        }
    }
}