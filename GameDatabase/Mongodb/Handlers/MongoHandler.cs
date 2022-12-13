using GameDatabase.Mongodb.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameDatabase.Mongodb.Handlers
{
    public class MongoHandler<T> : IGameDB<T> where T : class
    {
        private IMongoDatabase _database;
        private IMongoCollection<T> _collection;

        public MongoHandler(IMongoDatabase database)
        {
            _database = database;
            SetCollection();
        }

        private void SetCollection()
        {
            switch(typeof(T).Name)
            {
                case "User":
                    _collection = _database.GetCollection<T>("Users");
                    break;
                case "Room":
                    _collection = _database.GetCollection<T>("Room");
                    break;
            }
        }

        public List<T> GetAll()
        {
            var filter = Builders<T>.Filter.Empty;
            return _collection.Find(filter).ToList();
        }

        public IMongoDatabase GetDatabase()
        {
            return _database;
        }

        public T Get(FilterDefinition<T> filter)
        {
            return _collection.Find(filter).FirstOrDefault();
        }

        public void Remove(FilterDefinition<T> filter)
        {
            _collection.DeleteOne(filter);
        }

        public T Update(FilterDefinition<T> filter, UpdateDefinition<T> updater)
        {
            _collection.UpdateOne(filter, updater);
            return Get(filter);
        }

        public T Create(T item)
        {
            _collection.InsertOne(item);
            return item;
        }
    }
}
