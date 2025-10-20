using System.Buffers;
using System.Net.Sockets;
using MessagePack;
using SpellBreakers_Server.Packet;

namespace SpellBreakers_Server.Tcp
{
    public static class TcpPacketHelper
    {
        public static async Task SendAsync<T>(Socket socket, T packet) where T : PacketBase
        {
            byte[] header = new byte[6];
            byte[] body = MessagePackSerializer.Serialize(packet);

            BitConverter.GetBytes(packet.ID).CopyTo(header, 0);
            BitConverter.GetBytes(body.Length).CopyTo(header, 2);

            ArraySegment<byte>[] buffer = [new ArraySegment<byte>(header), new ArraySegment<byte>(body)];

            await socket.SendAsync(buffer, SocketFlags.None);
        }

        public static async Task<PacketBase?> ReceiveAsync(Socket socket)
        {
            byte[] header = new byte[6];

            int got = await socket.ReceiveAsync(header.AsMemory(), SocketFlags.None);
            if (got == 0) return null;

            ushort id = BitConverter.ToUInt16(header, 0);
            int length = BitConverter.ToInt32(header, 2);

            byte[] body = ArrayPool<byte>.Shared.Rent(length);

            try
            {
                int offset = 0;

                while (offset < length)
                {
                    int received = await socket.ReceiveAsync(body.AsMemory(offset, length - offset), SocketFlags.None);
                    if (received == 0) return null;

                    offset += received;
                }

                Type type = PacketRegistry.GetTypeById(id);

                return (PacketBase?)MessagePackSerializer.Deserialize(type, body.AsMemory(0, length));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(body);
            }
        }
    }
}
