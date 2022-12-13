namespace ChessGameServer.Applications.Messaging
{
    public enum WsTags
    {
        Invalid,
        Login,  
        Register,
        UserInfo,
        RoomInfo,
        CreateRoom,
        StartGame,
        FindGame,
        MovePiece,
        EndGame,
        RoomList,
        JoinRoom,
        ExitRoom,
        RestartGame,
    }
}