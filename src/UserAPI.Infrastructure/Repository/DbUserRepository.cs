using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using UserAPI.Application.Common.Abstraction.Repository;
using UserAPI.Domain.Entity;

namespace UserAPI.Infrastructure.Repository
{
    public class DbUserRepository : IUserRepository
    {
        private readonly IMongoDatabase _database;

        public DbUserRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task<bool> CreateUserAsync(UserEntity entity)
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

        public async Task<UserEntity> GetUserAsync(string login)
        {
            var collection = _database.GetCollection<UserEntity>("users");
            var builder = Builders<UserEntity>.Filter;
            var filter = builder.Eq(i => i.Username, login) | builder.Eq(i => i.Email, login);
            
            var user = await collection.Find(filter).FirstOrDefaultAsync();
            return user;
        }
    }
}