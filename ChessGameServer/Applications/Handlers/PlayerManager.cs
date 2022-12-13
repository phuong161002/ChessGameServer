using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ChessGameServer.Applications.Interfaces;
using ChessGameServer.Applications.Messaging.Constants;
using ChessGameServer.Logging;

namespace ChessGameServer.Applications.Handlers
{
    public class PlayerManager : IPlayerManager
    {
        public ConcurrentDictionary<string, IPlayer> Players { get; set; }
        private readonly IGameLogger _logger;

        public PlayerManager(IGameLogger logger)
        {
            Players = new ConcurrentDictionary<string, IPlayer>();
            _logger = logger;
        }

        public IPlayer FindPlayer(string sessionId)
        {
            return Players.FirstOrDefault(player => player.Key.Equals(sessionId)).Value;
        }

        public IPlayer FindPlayer(IPlayer player)
        {
            return Players.FirstOrDefault(item => item.Value.Equals(player)).Value;
        }

        public void AddPlayer(IPlayer player)
        {
            if (FindPlayer(player) == null)
            {
                Players.TryAdd(player.SessionId, player);
                _logger.Info($"List Player {Players.Count}");
            }
        }

        public void RemovePlayer(IPlayer player)
        {
            this.RemovePlayer(player.SessionId);
        }

        public void RemovePlayer(string id)
        {
            if (FindPlayer(id) != null)
            {
                Players.TryRemove(id, out var player);
                if (player != null)
                {
                    _logger.Info($"Remove player {player.SessionId} success");
                    _logger.Info($"List player {Players.Count}");
                }
            }
        }

        public bool HasUser(string username)
        {
            foreach (var player in Players.Values)
            {
                if (player.GetUserInfo().Username == username)
                {
                    return true;
                }
            }

            return false;
        }

        public List<IPlayer> GetPlayers() => Players.Values.ToList();
    }
}