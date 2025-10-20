using System.Net.Sockets;
using SpellBreakers_Server.Packet;
using SpellBreakers_Server.Tcp;
using SpellBreakers_Server.Users;

namespace SpellBreakers_Server.PacketHandlers
{
    class ChatHandler : IPacketHandler
    {
        public async Task HandleAsync(Socket socket, PacketBase packet)
        {
            if (packet is ChatPacket chat)
            {
                User? user = UserManager.Instance.GetBySocket(socket);
                chat.Sender = user?.Nickname ?? "(unknown)";

                await TcpPacketHelper.SendAsync(socket, chat);
            }
        }
    }
}
