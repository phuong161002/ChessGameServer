namespace ChessGameServer.Applications.Interfaces
{
    public interface IWsGameServer
    {
        void StartServer();
        void StopServer();
        void RestartServer();
    }
}