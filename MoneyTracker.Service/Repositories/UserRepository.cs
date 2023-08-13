using MongoDB.Driver;
using MoneyTracker.Service.Entities;

namespace MoneyTracker.Service.Repositories
{
    public class UserRepository
    {
        private const string collectionName = "Users";

        private readonly IMongoCollection<User> dbCollection;

        private readonly FilterDefinitionBuilder<User> filterBuilder = Builders<User>.Filter;

        public UserRepository()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var database = mongoClient.GetDatabase("UsersDB");
            dbCollection = database.GetCollection<User>(collectionName);
        }

        public async Task<IReadOnlyCollection<User>> GetAllAsync()
        {
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
        }
    }
}