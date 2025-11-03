using System.Net.Sockets;
using SpellBreakers_Server.Packet;
using SpellBreakers_Server.Users;

namespace SpellBreakers_Server.PacketHandlers.Auth
{
    public class RegisterHandler : IPacketHandler
    {
        public async Task HandleAsync(Socket socket, PacketBase packet)
        {
            if (packet is RegisterPacket register)
            {
                await AuthenticationManager.Instance.RegisterAsync(socket, register);
            }
        }
    }
}
