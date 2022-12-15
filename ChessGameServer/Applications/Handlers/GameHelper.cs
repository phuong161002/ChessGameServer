using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ChessGameServer.Applications.Handlers
{
    public static class GameHelper
    {
        public static string RandomString(int length)
        {
            var rdn = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid() + $"{DateTime.Now}"));
            return rdn[..length];
        }

        public static string ParseString<T>(T data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public static T ParseStruct<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public static string HashPassword(string txt)
        {
            var crypt = SHA256Managed.Create();
            var hash = string.Empty;
            var bytes = crypt.ComputeHash(Encoding.UTF8.GetBytes(txt));
            return bytes.Aggregate(hash, (current, theByte) => current + theByte.ToString("x2"));
        }
    }
}