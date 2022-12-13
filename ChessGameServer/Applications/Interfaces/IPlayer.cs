using ChessGameServer.Applications.Messaging;
using ChessGameServer.Applications.Messaging.Constants;
using ChessGameServer.Rooms.Handlers;

namespace ChessGameServer.Applications.Interfaces
{
    public interface IPlayer
    {
        string SessionId { get; set; }
        string Name { get; set; }
        BaseRoom Room { get; set; }
        PlayerState State { get; set; }
        void SetDisconnect(bool value);
        bool SendMessage(string msg);
        bool SendMessage<T>(WsMessage<T> message);
        void OnDisconnect();
        UserInfo GetUserInfo();
    }
}