using System;
using System.Diagnostics;
using System.Text;
using ChessGameServer.Applications.Interfaces;
using ChessGameServer.Applications.Messaging;
using ChessGameServer.Applications.Messaging.Constants;
using ChessGameServer.GameModels;
using ChessGameServer.GameModels.Base;
using ChessGameServer.Logging;
using GameDatabase.Mongodb.Interfaces;
using GameDatabase.Mongodb.Handlers;
using MongoDB.Driver;
using NetCoreServer;
using ChessGameServer.GameModels.Handlers;
using ChessGameServer.Rooms;
using ChessGameServer.Rooms.Handlers;
using MongoDB.Libmongocrypt;

namespace ChessGameServer.Applications.Handlers
{
    public class Player : WsSession, IPlayer
    {
        public string SessionId { get; set; }
        public string Name { get; set; }
        private readonly IGameLogger _logger;
        private UserHandler UsersDb { get; set; }
        private User UserInfo { get; set; }
        public BaseRoom Room { get; set; }
        public PlayerState State { get; set; }

        private bool IsDisconnected { get; set; }

        public Player(WsServer server, IMongoDatabase database) : base(server)
        {
            SessionId = this.Id.ToString();
            IsDisconnected = false;
            _logger = new GameLogger();
            UsersDb = new UserHandler(database);
        }

        public override void OnWsConnected(HttpRequest request)
        {
            IsDisconnected = false;
            var url = request.Url;
            _logger.Info("Player connected");
            State = PlayerState.Authenticate;
            base.OnWsConnected(request);
        }

        public override void OnWsDisconnected()
        {
            OnDisconnect();
            base.OnWsDisconnected();
        }

        public override void OnWsReceived(byte[] buffer, long offset, long size)
        {
            string msg = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            OnMessageReceived(msg);

            //((WsGameServer)Server).SendAll($"{SessionId} send message {msg}");
            // base.OnWsReceived(buffer, offset, size);
        }

        public void SetDisconnect(bool value)
        {
            IsDisconnected = value;
        }

        public bool SendMessage(string msg)
        {
            Console.WriteLine("Send msg " + msg);
            return this.SendTextAsync(msg);
        }

        public void OnDisconnect()
        {
            if (Room != null)
            {
                if (Room is PlayRoom)
                {
                    PlayerExitPlayRoom();
                }
                Room.ExitRoom(this);
            }

            IsDisconnected = true;
            _logger.Warning("Player disconnected", null);
        }

        private void OnMessageReceived(string msg)
        {
            try
            {
                var wsMes = GameHelper.ParseStruct<WsMessage<object>>(msg);
                Console.WriteLine(msg);
                switch (wsMes.Tags)
                {
                    case WsTags.Invalid:
                        break;
                    case WsTags.Login:
                        var loginData = GameHelper.ParseStruct<LoginData>(wsMes.Data.ToString());
                        OnPlayerLogin(loginData);
                        break;
                    case WsTags.Register:
                        var regData = GameHelper.ParseStruct<RegisterData>(wsMes.Data.ToString());
                        OnPlayerRegister(regData);
                        break;
                    case WsTags.RoomInfo:
                        break;
                    case WsTags.StartGame:
                        OnStartGame();
                        break;
                    case WsTags.FindGame:
                        break;
                    case WsTags.CreateRoom:
                        var room = ((WsGameServer)Server).RoomManager.CreateRoom(RoomType.Playing);
                        ((PlayRoom)room).State = PlayRoomState.Waiting;
                        Room?.ExitRoom(this);
                        PlayerJoinPlayRoom(room.Id);
                        break;
                    case WsTags.RoomList:
                        ((WsGameServer)Server).RoomManager.RoomListInfo();
                        break;
                    case WsTags.JoinRoom:
                        var roomId = wsMes.Data.ToString();
                        Room?.ExitRoom(this);
                        PlayerJoinPlayRoom(roomId);
                        break;
                    case WsTags.MovePiece:
                        var moveData = GameHelper.ParseStruct<MoveData>(wsMes.Data.ToString());
                        OnMovePiece(moveData);
                        break;
                    case WsTags.RestartGame:
                        State = PlayerState.WaitingForGame;

                        // Check if has an player are not ready
                        bool flag = false;
                        foreach (var player in Room.Players.Values)
                        {
                            if (player.State != PlayerState.WaitingForGame)
                            {
                                flag = true;
                                break;
                            }
                        }

                        // If all player are ready so room state = Waiting and new game can be started
                        if (!flag)
                        {
                            ((PlayRoom)Room).State = PlayRoomState.Waiting;
                        }
                        break;
                    case WsTags.EndGame:
                        if (Room is PlayRoom playRoom)
                        {
                            playRoom.State = PlayRoomState.Restarting;
                        }
                        break;
                    case WsTags.ExitRoom:
                        if (Room is PlayRoom)
                        {
                            PlayerExitPlayRoom();
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OnPlayerLogin(LoginData loginData)
        {
            bool flag = ((WsGameServer)Server).PlayerManager.HasUser(loginData.Username);

            if (UserInfo != null)
            {
                var msg = new WsMessage<string>(WsTags.Invalid, "You are logged in");
                this.SendMessage(GameHelper.ParseString(msg));
                return;
            }

            UserInfo = UsersDb.FindByUserName(loginData.Username);
            if (UserInfo != null)
            {
                var hashPass = GameHelper.HashPassword(loginData.Password);
                if (hashPass == UserInfo.Password)
                {
                    // Check if user is logged in another place
                    if (flag)
                    {
                        UserInfo = null;
                        var msg = new WsMessage<string>(WsTags.Invalid, "Your account is online in another place");
                        SendMessage(msg);
                        return;
                    }

                    // Login successfully
                    var msgInfo = new WsMessage<UserInfo>(WsTags.UserInfo, this.GetUserInfo());
                    SendMessage(msgInfo);
                    PlayerJoinLobby();
                    return;
                }
            }

            UserInfo = null;
            var invalidMsg = new WsMessage<string>(WsTags.Invalid, "Username or Password is Invalid");
            this.SendMessage(invalidMsg);
        }

        private void OnPlayerRegister(RegisterData registerData)
        {
            if (UserInfo != null)
            {
                var invalidMsg = new WsMessage<string>(WsTags.Invalid, "You are logged in");
                this.SendMessage(invalidMsg);
                return;
            }

            var check = UsersDb.FindByUserName(registerData.Username);
            if (check != null)
            {
                var invalidMsg = new WsMessage<string>(WsTags.Invalid, "Username existed");
                this.SendMessage(invalidMsg);
                return;
            }

            var newUser = new User(registerData.Username, registerData.Password, registerData.DisplayName);
            UserInfo = UsersDb.Create(newUser);
            if (UserInfo != null)
            {
                // Create new user successfully
                var msgInfo = new WsMessage<UserInfo>(WsTags.UserInfo, this.GetUserInfo());
                SendMessage(msgInfo);
                PlayerJoinLobby();
            }
        }

        private void PlayerJoinLobby()
        {
            var lobby = ((WsGameServer)Server).RoomManager.Lobby;
            lobby.JoinRoom(this);
            State = PlayerState.Lobby;
        }

        private void PlayerJoinPlayRoom(string roomId)
        {
            var playRoom = ((WsGameServer)Server).RoomManager.FindRoom(roomId);
            if (!(playRoom is PlayRoom))
            {
                return;
            }

            ((PlayRoom)playRoom).JoinRoom(this);
            State = PlayerState.WaitingForGame;
        }

        private void PlayerExitPlayRoom()
        {
            var exitRoomMsg = new WsMessage<object>(WsTags.ExitRoom, "Exit OK");
            SendMessage(exitRoomMsg);
            ((PlayRoom)Room).State = PlayRoomState.Waiting;
            Room.ExitRoom(this);
            PlayerJoinLobby();
        }

        public UserInfo GetUserInfo()
        {
            if (UserInfo != null)
            {
                return new UserInfo()
                {
                    Username = UserInfo.Username,
                    DisplayName = UserInfo.DisplayName,
                    Amount = UserInfo.Amount,
                    Level = UserInfo.Level,
                    Avatar = UserInfo.Avatar
                };
            }

            return default;
        }

        public bool SendMessage<T>(WsMessage<T> message)
        {
            return SendMessage(GameHelper.ParseString(message));
        }

        private void OnMovePiece(MoveData moveData)
        {
            var opponent = ((PlayRoom)Room).FindOpponent(this);
            var msg = new WsMessage<MoveData>(WsTags.MovePiece, moveData);
            opponent.SendMessage(msg);
        }

        private void OnStartGame()
        {
            if (!(Room is PlayRoom playRoom))
            {
                var invalidMsg = new WsMessage<string>(WsTags.Invalid, "You are not in play room");
                SendMessage(invalidMsg);
            }
            else if (Room == null || Room.Players.Count < 2)
            {
                var invalidMsg = new WsMessage<string>(WsTags.Invalid, "Not enough player");
                SendMessage(invalidMsg);
            }
            else if (playRoom.State == PlayRoomState.Restarting)
            {
                var invalidMsg = new WsMessage<string>(WsTags.Invalid, "Your opponent is not ready");
                SendMessage(invalidMsg);
            }
            else if (playRoom.State == PlayRoomState.Playing)
            {
                var invalidMsg = new WsMessage<string>(WsTags.Invalid, "The game has already started");
                SendMessage(invalidMsg);
            }
            else
            {
                var startGameData = new StartGameData() { MyTeamColor = TeamColor.WHITE };
                var opponent = playRoom.FindOpponent(this);
                var opponentStartGameData = new StartGameData() { MyTeamColor = TeamColor.BLACK };
                var myMsg = new WsMessage<StartGameData>(WsTags.StartGame, startGameData);
                var opponentMsg = new WsMessage<StartGameData>(WsTags.StartGame, opponentStartGameData);
                SendMessage(myMsg);
                opponent.SendMessage(opponentMsg);
                playRoom.State = PlayRoomState.Playing;
                foreach (var player in playRoom.Players.Values)
                {
                    player.State = PlayerState.Playing;
                }
            }
        }
    }
}