using System.Net.Sockets;
using SpellBreakers_Server.PacketHandlers;

namespace SpellBreakers_Server.Packet
{
    public static class PacketHandler
    {
        private static readonly Dictionary<PacketId, IPacketHandler> _handlers = new Dictionary<PacketId, IPacketHandler>();

        public static void Register(PacketId id, IPacketHandler handler)
        {
            _handlers[id] = handler;
        }

        public static async void Handle(Socket socket, PacketBase packet)
        {
            if(_handlers.TryGetValue((PacketId)packet.ID, out IPacketHandler? handler))
            {
                await handler.HandleAsync(socket, packet);
            }
            else
            {
                Console.WriteLine($"[서버] 할당되지 않은 Packet Id - {(PacketId)packet.ID}");
            }
        }
    }
}
