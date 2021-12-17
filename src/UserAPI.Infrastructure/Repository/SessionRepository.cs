using System.Threading.Tasks;
using MongoDB.Driver;
using UserAPI.Application.Common.Abstraction.Repository;
using UserAPI.Domain.Entity;

namespace UserAPI.Infrastructure.Repository
{
    public class SessionRepository : ISessionsRepository
    {
        private readonly IMongoDatabase _database;

        public SessionRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task<SessionEntity> GetAsync(string id)
        {
            var collection = _database.GetCollection<SessionEntity>("sessions");
            var builder = Builders<SessionEntity>.Filter;
            var filter = builder.Eq(i => i.Id, id);
            
            var session = await collection.Find(filter).FirstOrDefaultAsync();
            return session;
        }

        public async Task CreateAsync(SessionEntity entity)
        {
            var collection = _database.GetCollection<SessionEntity>("sessions");
            
            await collection.InsertOneAsync(entity);
        }
    }
}