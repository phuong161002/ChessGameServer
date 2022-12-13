using MongoDB.Driver;
using System.Collections.Generic;

namespace GameDatabase.Mongodb.Interfaces
{
    public interface IGameDB<T> where T : class
    {
        IMongoDatabase GetDatabase();
        T Get(FilterDefinition<T> filter);
        List<T> GetAll();
        T Create(T item);
        void Remove(FilterDefinition<T> filter);
        T Update(FilterDefinition<T> filter, UpdateDefinition<T> updater);

    }
}