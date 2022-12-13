
using MongoDB.Driver;

namespace GameDatabase.Mongodb.Handlers
{
    public class MongoDb
    {
        private readonly IMongoClient _client;
        private IMongoDatabase _database => _client.GetDatabase("ChessOnline");



        public MongoDb()
        {
            var setting = MongoClientSettings.FromConnectionString("mongodb://localhost:27017/");
            _client = new MongoClient(setting);
        }

        public IMongoDatabase GetDatabase()
        {
            return _database;
        }
    }
}