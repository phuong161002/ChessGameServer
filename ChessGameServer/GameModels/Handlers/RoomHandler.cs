using ChessGameServer.GameModels.Interfaces;
using GameDatabase.Mongodb.Handlers;
using GameDatabase.Mongodb.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGameServer.GameModels.Handlers
{
    public class RoomHandler : IDbHandler<RoomModel>
    {
        private readonly IGameDB<RoomModel> _roomDb;

        public RoomHandler(IMongoDatabase database)
        {
            _roomDb = new MongoHandler<RoomModel>(database);
        }

        public RoomModel Create(RoomModel item)
        {
            throw new NotImplementedException();
        }

        public RoomModel Find(string id)
        {
            throw new NotImplementedException();
        }

        public List<RoomModel> FindAll()
        {
            throw new NotImplementedException();
        }

        public void Remove(string id)
        {
            throw new NotImplementedException();
        }

        public RoomModel Update(string id, RoomModel item)
        {
            throw new NotImplementedException();
        }
    }
}
