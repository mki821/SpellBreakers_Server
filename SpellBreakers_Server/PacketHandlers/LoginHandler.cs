using System.Net.Sockets;
using SpellBreakers_Server.Packet;
using SpellBreakers_Server.Users;

namespace SpellBreakers_Server.PacketHandlers
{
    public class LoginHandler : IPacketHandler
    {
        public async Task HandleAsync(Socket socket, PacketBase packet)
        {
            if (packet is LoginPacket login)
            {
                await AuthenticationManager.Instance.LoginAsync(socket, login);
            }
        }
    }
}
