using System.Net.Sockets;
using SpellBreakers_Server.Packet;
using SpellBreakers_Server.Users;

namespace SpellBreakers_Server.PacketHandlers.Auth
{
    public class AutoLoginHandler : IPacketHandler
    {
        public async Task HandleAsync(Socket socket, PacketBase packet)
        {
            if(packet is AutoLoginPacket login)
            {
                await AuthenticationManager.Instance.AutoLoginAsync(socket, login);
            }
        }
    }
}
