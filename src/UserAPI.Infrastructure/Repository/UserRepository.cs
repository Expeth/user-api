using System.Threading.Tasks;
using MongoDB.Driver;
using UserAPI.Application.Common.Abstraction.Repository;
using UserAPI.Domain.Entity;

namespace UserAPI.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoDatabase _database;

        public UserRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task<bool> CreateAsync(UserEntity entity)
        {
            var collection = _database.GetCollection<UserEntity>("users");
            var duplicate = await collection.Find(i =>
                    i.Email == entity.Email ||
                    i.Username == entity.Username)
                .FirstOrDefaultAsync();

            if (duplicate != null) return false;
            
            await collection.InsertOneAsync(entity);
            return true;
        }

        public async Task<UserEntity> GetAsync(string login)
        {
            var collection = _database.GetCollection<UserEntity>("users");
            var builder = Builders<UserEntity>.Filter;
            var filter = builder.Eq(i => i.Username, login) | builder.Eq(i => i.Email, login);
            
            var user = await collection.Find(filter).FirstOrDefaultAsync();
            return user;
        }
    }
}