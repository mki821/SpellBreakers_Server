using System.Net.Sockets;
using SpellBreakers_Server.Packet;
using SpellBreakers_Server.Rooms;
using SpellBreakers_Server.Users;

namespace SpellBreakers_Server.PacketHandlers.Rooms
{
    public class CreateRoomHandler : IPacketHandler
    {
        public async Task HandleAsync(Socket socket, PacketBase packet)
        {
            if(packet is CreateRoomPacket create)
            {
                Room room = new Room(create.Name, create.Password);
                RoomManager.Instance.Add(room);

                User? user = UserManager.Instance.GetBySocket(socket);
                if (user == null) return;

                await room.TryJoin(user, create.Password);
            }
        }
    }
}
