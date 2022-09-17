using MongoDB.Driver;

namespace UserApi.Repository
{
    public class Context
    {
        private readonly IMongoDatabase _database;
        
        public Context(IMongoClient mongoClient) 
        {
            _database = mongoClient.GetDatabase("UserAPI");
        }

        
        public IMongoCollection<T> GetCollection<T>(object collectionName) 
        {
            return _database.GetCollection<T>(collectionName.ToString());
        }
        
    }
}