using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGameServer.Applications.Messaging.Constants
{
    public struct UserInfo
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Avatar { get; set; }
        public int Level { get; set; }
        public long Amount { get; set; }
    }
}
