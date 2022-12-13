using ChessGameServer.Rooms.Handlers;
using System;
using System.Collections.Generic;
using System.Text;
using ChessGameServer.Applications.Messaging.Constants;
using ChessGameServer.GameModels.Base;

namespace ChessGameServer.Rooms.Interfaces
{
    public interface IRoomManager
    {
        BaseRoom Lobby { get; set; }
        BaseRoom CreateRoom(RoomType roomType);
        BaseRoom FindRoom(string id);
        bool RemoveRoom(string id);
        void RoomListInfo();
    }
}
