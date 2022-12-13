using System;
using System.Net;
using ChessGameServer.Applications.Handlers;
using ChessGameServer.Applications.Interfaces;
using ChessGameServer.Logging;
using ChessGameServer.Rooms.Handlers;
using ChessGameServer.Rooms.Interfaces;
using GameDatabase.Mongodb.Handlers;

namespace ChessGameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            IGameLogger logger = new GameLogger();
            var mongodb = new MongoDb();
            IPlayerManager playerManager = new PlayerManager(logger);
            IRoomManager roomManager = new RoomManager();
            var wsServer = new WsGameServer(IPAddress.Any, 3000, playerManager, logger, mongodb, roomManager);
            wsServer.StartServer();
            logger.Print("Game Server Started");
            for (;;)
            {
                var type = Console.ReadLine();
                if (type == "restart")
                {
                    logger.Print("Game Server restarting....");
                    wsServer.Restart();
                }
                else if (type == "shutdown")
                {
                    logger.Print("Game Server stopping....");
                    wsServer.StopServer();
                    break;
                }
            }
        }
    }
}