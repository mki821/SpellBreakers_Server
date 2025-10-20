using System.Net.Sockets;
using SpellBreakers_Server.Packet;
using SpellBreakers_Server.Users;

namespace SpellBreakers_Server.PacketHandlers
{
    public class MoveHandler : IPacketHandler
    {
        public async Task HandleAsync(Socket socket, PacketBase packet)
        {
            if (packet is MovePacket move)
            {
                User? user = UserManager.Instance.GetBySocket(socket);
                if (user == null) throw new Exception("Not Existent User!");

                Console.WriteLine($"[테스트] {user.Nickname} Move X = {move.X}, Y = {move.Y}");
            }
        }
    }
}
