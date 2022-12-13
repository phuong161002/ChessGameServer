using System;
using System.Collections.Generic;
using System.Text;
using ChessGameServer.GameModels.Base;

namespace ChessGameServer.Applications.Messaging.Constants
{
    public struct RoomInfo
    {
        public RoomType RoomType { get; set; }
        public string Id { get; set; }
        public List<UserInfo> Players { get; set; }
    }
}
