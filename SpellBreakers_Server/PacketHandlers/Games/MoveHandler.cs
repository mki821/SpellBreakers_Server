using System.Net.Sockets;
using SpellBreakers_Server.Packet;
using SpellBreakers_Server.Users;

namespace SpellBreakers_Server.PacketHandlers.Games
{
    public class MoveHandler : IPacketHandler
    {
        public async Task HandleAsync(Socket socket, PacketBase packet)
        {
            if (packet is MovePacket move)
            {
                User? user = UserManager.Instance.GetBySocket(socket);
                if (user == null) return;

                user.CurrentRoom?.Game.Move(move);
            }
        }
    }
}
