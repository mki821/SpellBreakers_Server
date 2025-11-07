using System.Net.Sockets;
using SpellBreakers_Server.Packet;
using SpellBreakers_Server.Users;

namespace SpellBreakers_Server.PacketHandlers.Auth
{
    public class UdpConnectHandler : IPacketHandler
    {
        public Task HandleAsync(Socket socket, PacketBase packet)
        {
            User? user = UserManager.Instance.GetBySocket(socket);
            if (user != null)
            {
                Console.WriteLine($"[서버] UDP 연결! : {user.Nickname}");
            }

            return Task.CompletedTask;
        }
    }
}
