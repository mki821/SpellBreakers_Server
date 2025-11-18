using System.Net.Sockets;
using SpellBreakers_Server.Packet;
using SpellBreakers_Server.Users;

namespace SpellBreakers_Server.PacketHandlers.Games
{
    public class SkillHandler : IPacketHandler
    {
        public Task HandleAsync(Socket socket, PacketBase packet)
        {
            if (packet is SkillPacket skill)
            {
                User? user = UserManager.Instance.GetBySocket(socket);
                if (user == null) return Task.CompletedTask;

                user.CurrentRoom?.Game.UseSkill(skill);
            }

            return Task.CompletedTask;
        }
    }
}
