using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using ChessGameServer.Applications.Interfaces;
using ChessGameServer.Applications.Messaging;
using ChessGameServer.GameModels.Base;
using ChessGameServer.Logging;
using ChessGameServer.Rooms.Interfaces;
using GameDatabase.Mongodb.Handlers;
using MongoDB.Driver;
using NetCoreServer;

namespace ChessGameServer.Applications.Handlers
{
    public class WsGameServer : WsServer, IWsGameServer
    {
        public const int MaxRoom = 10;
        
        private readonly int _port;
        public readonly IPlayerManager PlayerManager;
        private readonly IGameLogger _logger;
        private readonly MongoDb _MongoDb;
        public readonly IRoomManager RoomManager;
        
        
        public WsGameServer(IPAddress address, int port, IPlayerManager playerManager, IGameLogger logger,
            MongoDb mongodb,  IRoomManager roomManager) : base(address, port)
        {
            this._port = port;
            this.PlayerManager = playerManager;
            this._logger = logger;
            this._MongoDb = mongodb;
            this.RoomManager = roomManager;
        }

        public void StartServer()
        {
            if (this.Start())
            {
                _logger.Print($"Server started at port {_port}");
            }
        }

        protected override void OnError(SocketError error)
        {
            _logger.Error($"Server Ws error");
            base.OnError(error);
        }

        protected override void OnDisconnected(TcpSession session)
        {
            IPlayer player = PlayerManager.FindPlayer(session.Id.ToString());
            if (player != null)
            {
                PlayerManager.RemovePlayer(player);
                //todo mark player as disconnected
            }
            _logger.Info("Session disconnected");
            base.OnDisconnected(session);
        }

        public void StopServer()
        {
            //todo something before stop server
            this.Stop();
            _logger.Print("Server Ws Stopped");
        }

        public void RestartServer()
        {
            //todo something before restart server
            this.Restart();
            _logger.Print("Server Ws Restarted");
        }

        protected override TcpSession CreateSession()
        {
            _logger.Info("New session created");
            Player player = new Player(this, _MongoDb.GetDatabase());
            PlayerManager.AddPlayer(player);
            player.SendMessage(new WsMessage<object>(WsTags.Invalid, null));
            return player;
        }

        public void SendAll(string msg)
        {
            this.MulticastText(msg);
        }
    }
}