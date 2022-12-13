using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ChessGameServer.Applications.Interfaces
{
    public interface IPlayerManager
    {
        ConcurrentDictionary<string, IPlayer> Players { get; set; }

        IPlayer FindPlayer(string sessionId);
        IPlayer FindPlayer(IPlayer player);
        void AddPlayer(IPlayer player);
        void RemovePlayer(IPlayer player);
        void RemovePlayer(string id);

        bool HasUser(string username);
        List<IPlayer> GetPlayers();
    }
}