namespace ChessGameServer.Applications.Messaging
{
    public interface IMessage<T>
    {
        WsTags Tags { get; set; }
        T Data { get; set; }
    }
}