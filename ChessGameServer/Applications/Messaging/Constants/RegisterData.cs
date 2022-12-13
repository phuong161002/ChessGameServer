using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGameServer.Applications.Messaging.Constants
{
    public struct RegisterData
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
    }
}
