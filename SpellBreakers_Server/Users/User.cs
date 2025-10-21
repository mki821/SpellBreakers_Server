using System.Net;
using System.Net.Sockets;
using SpellBreakers_Server.Rooms;

namespace SpellBreakers_Server.Users
{
    public class User
    {
        public string Token { get; }

        public string? Nickname { get; set; }

        public Socket TcpSocket { get; set; }
        public EndPoint? UdpEndPoint { get; set; }

        public Room? CurrentRoom { get; set; }

        public User(string token, string nickname, Socket socket)
        {
            Token = token;
            Nickname = nickname;
            TcpSocket = socket;
        }
    }
}
