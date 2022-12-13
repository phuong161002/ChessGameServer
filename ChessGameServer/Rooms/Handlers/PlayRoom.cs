using System;
using System.Diagnostics;
using System.Linq;
using ChessGameServer.Applications.Handlers;
using ChessGameServer.Applications.Interfaces;
using ChessGameServer.GameModels.Base;

namespace ChessGameServer.Rooms.Handlers
{
    public class PlayRoom : BaseRoom
    {
        const int MaxPlayer = 2;
        public PlayRoomState State { get; set; }

        public PlayRoom() : base()
        {
            RoomType = RoomType.Playing;
        }

        public new bool JoinRoom(IPlayer player)
        {
            Console.WriteLine($"Player {player.SessionId} join Playroom");
            
            if (Players.Count >= MaxPlayer)
            {
                return false;
            }

            return base.JoinRoom(player);
        }

        public IPlayer FindOpponent(IPlayer player)
        {
            return Players.FirstOrDefault(p => p.Key != player.SessionId).Value;
        }
    }
}