using ChessGameServer.Applications.Interfaces;
using ChessGameServer.Applications.Messaging;
using ChessGameServer.Rooms.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ChessGameServer.Applications.Handlers;
using ChessGameServer.Applications.Messaging.Constants;
using ChessGameServer.GameModels.Base;

namespace ChessGameServer.Rooms.Handlers
{
    public class BaseRoom : IBaseRoom
    {
        public string Id { get; set; }
        public RoomType RoomType { get; set; }
        public ConcurrentDictionary<string, IPlayer> Players { get; set; }
        public event Action OnPlayerJoin;
        public event Action OnPlayerExit;
        

        public BaseRoom()
        {
            Id = GameHelper.RandomString(10);
            RoomType = RoomType.BaseRoom;
            Players = new ConcurrentDictionary<string, IPlayer>();
        }

        public bool ExitRoom(IPlayer player)
        {
            return ExitRoom(player.SessionId);
        }

        public bool ExitRoom(string id)
        {
            var player = FindPlayer(id);
            if (player != null)
            {
                Players.TryRemove(player.SessionId, out player);
                if (this is PlayRoom)
                {
                    player.Room = null;
                }
                RoomInfo();
                OnPlayerExit?.Invoke();
                return true;
            }
            return false;
        }

        public IPlayer FindPlayer(string id)
        {
            return Players.FirstOrDefault(p => p.Key.Equals(id)).Value;
        }

        public bool JoinRoom(IPlayer player)
        {
            if (FindPlayer(player.SessionId) == null)
            {
                if (Players.TryAdd(player.SessionId, player))
                {
                    player.Room = this;
                    RoomInfo();
                    OnPlayerJoin?.Invoke();
                }
            }

            return false;
        }

        public void SendMessage(string msg)
        {
            lock (Players)
            {
                foreach (var player in Players.Values)
                {
                    player.SendMessage(msg);
                }
            }
        }

        public void SendMessage<T>(WsMessage<T> message)
        {
            lock (Players)
            {
                foreach (var player in Players.Values)
                {
                    player.SendMessage(message);
                }
            }
        }

        public void SendMessage<T>(WsMessage<T> message, string IdIgnore)
        {
            lock (Players)
            {
                foreach (var player in Players.Values.Where(p => p.SessionId != IdIgnore))
                {
                    player.SendMessage(message);
                }
            }
        }

        private void RoomInfo()
        {
            var roomInfo = new RoomInfo()
            {
                Id = Id,
                RoomType = RoomType,
                Players = Players.Values.Select(p => p.GetUserInfo()).ToList(),
            };
            var msg = new WsMessage<RoomInfo>(WsTags.RoomInfo, roomInfo);
            SendMessage(msg);
        }

        public RoomInfo GetRoomInfo()
        {
            var roomInfo = new RoomInfo()
            {
                Id = Id,
                RoomType = RoomType,
                Players = Players.Values.Select(p => p.GetUserInfo()).ToList(),
            };
            return roomInfo;
        }
    }
}
