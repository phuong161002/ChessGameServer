using ChessGameServer.Rooms.Handlers;
using System.Collections.Generic;

namespace ChessGameServer.Applications.Messaging.Constants
{
    public struct StartGameData
    {
        public string OpponentName { get; set; }
        public TeamColor MyTeamColor { get; set; }
    }

    public enum TeamColor
    {
        WHITE,
        BLACK
    }
}