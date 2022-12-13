using System.Collections.Generic;

namespace ChessGameServer.GameModels.Interfaces
{
    public interface IDbHandler<T>
    {
        T Find(string id);
        List<T> FindAll();
        T Create(T item);
        T Update(string id, T item);
        void Remove(string id);
    }
}
