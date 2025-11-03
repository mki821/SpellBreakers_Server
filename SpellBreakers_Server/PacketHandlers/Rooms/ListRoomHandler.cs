using System.Net.Sockets;
using SpellBreakers_Server.Packet;
using SpellBreakers_Server.Rooms;

namespace SpellBreakers_Server.PacketHandlers.Rooms
{
    public class ListRoomHandler : IPacketHandler
    {
        public async Task HandleAsync(Socket socket, PacketBase packet)
        {
            await RoomManager.Instance.GetRoomList(socket);
        }
    }
}
