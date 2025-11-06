using System.Net.Sockets;
using SpellBreakers_Server.Packet;
using SpellBreakers_Server.Users;

namespace SpellBreakers_Server.PacketHandlers.Rooms
{
    public class ReadyHandler : IPacketHandler
    {
        public async Task HandleAsync(Socket socket, PacketBase packet)
        {
            if (packet is ReadyPacket)
            {
                User? user = UserManager.Instance.GetBySocket(socket);
                if (user == null) return;
                if(user.CurrentRoom == null) return;

                await user.CurrentRoom.Ready(user);
            }
        }
    }
}
