using System.Net.Sockets;
using SpellBreakers_Server.Packet;
using SpellBreakers_Server.Tcp;
using SpellBreakers_Server.Users;

namespace SpellBreakers_Server.PacketHandlers
{
    public class SwitchRoleHnadler : IPacketHandler
    {
        public async Task HandleAsync(Socket socket, PacketBase packet)
        {
            User? user = UserManager.Instance.GetBySocket(socket);
            if (user == null) return;

            if(user.CurrentRoom != null)
            {
                await user.CurrentRoom.SwitchRole(user);
            }
            else
            {
                SwitchRoleResponsePacket response = new SwitchRoleResponsePacket();
                response.Success = false;
                response.Message = "방을 찾을 수 없습니다!";

                await TcpPacketHelper.SendAsync(socket, response);
            }
        }
    }
}
