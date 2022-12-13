using ChessGameServer.Rooms.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ChessGameServer.Applications.Messaging;
using ChessGameServer.Applications.Messaging.Constants;
using ChessGameServer.GameModels.Base;
using ZstdSharp.Unsafe;

namespace ChessGameServer.Rooms.Handlers
{
    public class RoomManager : IRoomManager
    {
        public BaseRoom Lobby { get; set; }
        private ConcurrentDictionary<string, BaseRoom> Rooms { get; set; }

        public RoomManager()
        {
            Rooms = new ConcurrentDictionary<string, BaseRoom>();
            Lobby = new LobbyRoom();
            Lobby.OnPlayerJoin += RoomListInfo;
            Lobby.OnPlayerExit += RoomListInfo;
        }

        public BaseRoom FindRoom(string id)
        {
            return Rooms.FirstOrDefault(r => r.Key.Equals(id)).Value;
        }

        public bool RemoveRoom(string id)
        {
            var oldRoom = FindRoom(id);
            if (oldRoom != null)
            {
                Rooms.TryRemove(id, out var room);
                
                RoomListInfo();
                
                return room != null;
            }

            return false;
        }

        public BaseRoom CreateRoom(RoomType roomType)
        {
            BaseRoom newRoom;
            switch (roomType)
            {
                case RoomType.Lobby:
                    newRoom = new LobbyRoom();
                    break;
                case RoomType.Playing:
                    newRoom = new PlayRoom();
                    break;
                default:
                    newRoom = new BaseRoom();
                    break;
            }

            if (newRoom is PlayRoom)
            {
                newRoom.OnPlayerExit += CheckRoomValid;
            }
            newRoom.OnPlayerExit += RoomListInfo;
            newRoom.OnPlayerJoin += RoomListInfo;
            Rooms.TryAdd(newRoom.Id, newRoom);
            
            RoomListInfo();
            
            return newRoom;
        }

        // Update room list on client
        public void RoomListInfo()
        {
            RoomList roomList = new RoomList
            {
                Rooms = new List<RoomInfo>()
            };
            foreach (var room in Rooms.Values)
            {
                roomList.Rooms.Add(room.GetRoomInfo());
            }

            var message = new WsMessage<RoomList>(WsTags.RoomList, roomList);
            Lobby.SendMessage(message);
        }

        public void CheckRoomValid()
        {
            List<BaseRoom> roomToRemove = new List<BaseRoom>();
            foreach (var room in Rooms.Values)
            {
                if (room.Players.Count <= 0)
                {
                    roomToRemove.Add(room);
                }
            }
            
            roomToRemove.ForEach(r => Rooms.Remove(r.Id, out BaseRoom br));
        }
    }
}