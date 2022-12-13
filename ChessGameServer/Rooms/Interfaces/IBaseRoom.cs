using ChessGameServer.Applications.Interfaces;
using ChessGameServer.Applications.Messaging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ChessGameServer.Rooms.Interfaces
{
    public interface IBaseRoom
    {
        public string Id { get; set; }
        public ConcurrentDictionary<string, IPlayer> Players { get; set; }

        bool JoinRoom(IPlayer player);
        bool ExitRoom(IPlayer player);
        bool ExitRoom(string id);
        IPlayer FindPlayer(string id);
        void SendMessage(string msg);
        void SendMessage<T>(WsMessage<T> message);
        void SendMessage<T>(WsMessage<T> message, string IdIgnore);

    }
}
