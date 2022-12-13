using ChessGameServer.GameModels.Base;

namespace ChessGameServer.Rooms.Handlers
{
    public class LobbyRoom : BaseRoom
    {
        public LobbyRoom() : base()
        {
            RoomType = RoomType.Lobby;
        }
    }
}