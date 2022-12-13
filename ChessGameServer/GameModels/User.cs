
using ChessGameServer.Applications.Handlers;

namespace ChessGameServer.GameModels
{
    public class User: Base.BaseModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string Avatar { get; set; }
        public int Level { get; set; }
        public long Amount { get; set; }

        public User(string username, string password, string displayName)
        {
            this.Username = username;
            this.Password = GameHelper.HashPassword(password);
            this.DisplayName = displayName;
            this.Avatar = "";
            this.Level = 1;
            this.Amount = 0;
        }
    }
}
