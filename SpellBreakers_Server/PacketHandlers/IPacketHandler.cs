using System.Net.Sockets;
using SpellBreakers_Server.Packet;

namespace SpellBreakers_Server.PacketHandlers
{
    public interface IPacketHandler
    {
        public Task HandleAsync(Socket socket, PacketBase packet);
    }
}
