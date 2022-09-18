using MongoDB.Driver;

namespace UserApi.Repository
{
    public class DBContext
    {
        private readonly IMongoDatabase _database;
        
        public DBContext(IMongoClient mongoClient) 
        {
            _database = mongoClient.GetDatabase("UserAPI");
        }

        
        public IMongoCollection<T> GetCollection<T>(object collectionName) 
        {
            return _database.GetCollection<T>(collectionName.ToString());
        }
        
    }
}