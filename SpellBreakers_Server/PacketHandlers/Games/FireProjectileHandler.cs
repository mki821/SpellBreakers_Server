using SpellBreakers_Server.Packet;
using SpellBreakers_Server.Users;
using System.Net.Sockets;

namespace SpellBreakers_Server.PacketHandlers.Games
{
    public class FireProjectileHandler : IPacketHandler
    {
        public Task HandleAsync(Socket socket, PacketBase packet)
        {
            if (packet is FireProjectilePacket fireProjectile)
            {
                User? user = UserManager.Instance.GetBySocket(socket);
                if (user == null) return Task.CompletedTask;

                user.CurrentRoom?.Game.FireProjectile(fireProjectile);
            }

            return Task.CompletedTask;
        }
    }
}
