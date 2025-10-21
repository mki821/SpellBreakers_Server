using System.Net.Sockets;
using SpellBreakers_Server.Packet;
using SpellBreakers_Server.Rooms;
using SpellBreakers_Server.Tcp;
using SpellBreakers_Server.Users;

namespace SpellBreakers_Server.PacketHandlers
{
    public class JoinRoomHandler : IPacketHandler
    {
        public async Task HandleAsync(Socket socket, PacketBase packet)
        {
            if(packet is JoinRoomPacket join)
            {
                Room? room = RoomManager.Instance.GetByID(join.RoomID);
                if (room == null)
                {
                    JoinRoomResponsePacket response = new JoinRoomResponsePacket();
                    response.Success = false;
                    response.Message = "방을 찾을 수 없습니다!";

                    await TcpPacketHelper.SendAsync(socket, response);
                }
                else
                {
                    User? user = UserManager.Instance.GetBySocket(socket);
                    if (user == null) return;

                    await room.TryJoin(user, join.Password);
                }
            }
        }
    }
}
