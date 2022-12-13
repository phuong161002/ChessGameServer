using System;

namespace ChessGameServer.Logging
{
    public interface IGameLogger
    {
        void Print(string msg);
        void Info(string info);
        void Warning(string warning, Exception exception);
        void Error(string error, Exception exception);
        void Error(string error);
    }
}