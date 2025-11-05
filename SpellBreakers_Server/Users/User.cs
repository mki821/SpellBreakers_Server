using System.Net;
using System.Net.Sockets;
using SpellBreakers_Server.Rooms;

namespace SpellBreakers_Server.Users
{
    public class User
    {
        public string Token { get; }

        public string ID { get; set; }
        public string Nickname { get; set; } = "(Unknown)";

        public Socket TcpSocket { get; set; }
        public EndPoint? UdpEndPoint { get; set; }

        public Room? CurrentRoom { get; set; }

        public User(string token, string id, string nickname, Socket socket)
        {
            Token = token;
            ID = id;
            Nickname = nickname;
            TcpSocket = socket;
        }
    }
}
