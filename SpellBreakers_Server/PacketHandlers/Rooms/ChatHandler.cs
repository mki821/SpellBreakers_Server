using System.Net.Sockets;
using SpellBreakers_Server.Packet;
using SpellBreakers_Server.Users;

namespace SpellBreakers_Server.PacketHandlers.Rooms
{
    public class ChatHandler : IPacketHandler
    {
        public async Task HandleAsync(Socket socket, PacketBase packet)
        {
            if (packet is ChatPacket chat)
            {
                User? user = UserManager.Instance.GetBySocket(socket);
                if (user == null) return;

                if(user.CurrentRoom != null)
                {
                    chat.Sender = user.Nickname ?? "(unknown)";

                    await user.CurrentRoom.Broadcast(chat);
                }
            }
        }
    }
}
