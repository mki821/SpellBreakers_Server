using System.Net.Sockets;
using SpellBreakers_Server.Packet;
using SpellBreakers_Server.Tcp;

namespace SpellBreakers_Server.Rooms
{
    public class RoomManager
    {
        private static Lazy<RoomManager> _instance = new Lazy<RoomManager>(() => new RoomManager());
        public static RoomManager Instance => _instance.Value;

        private Dictionary<ushort, Room> _roomsByName = new Dictionary<ushort, Room>();
        private ushort _currentId = 1;

        public async Task GetRoomList(Socket socket)
        {
            ListRoomResponsePacket packet = new ListRoomResponsePacket();

            foreach(Room room in _roomsByName.Values)
            {
                RoomElement element = new RoomElement
                {
                    Name = room.Name,
                    Locked = room.Password != "",
                    Playing = false,
                    Players = (ushort)room.PlayerCount,
                    Spectators = (ushort)room.SpectatorCount
                };
                packet.Rooms.Add(element);
            }

            await TcpPacketHelper.SendAsync(socket, packet);
        }

        public void Add(Room room)
        {
            room.ID = _currentId++;
            _roomsByName.Add(room.ID, room);
        }

        public Room? GetByID(ushort id)
        {
            _roomsByName.TryGetValue(id, out Room? room);
            return room;
        }
    }
}
